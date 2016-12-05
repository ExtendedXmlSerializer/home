// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
//                    Michael DeMond
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Common;

namespace ExtendedXmlSerialization.Write
{
    public interface IWriteInstruction<in T>
    {
        void Execute(IWriting services, T instance);
    }

    public class DecoratedWriteInstruction : DecoratedInstruction<IWriting>
    {
        public DecoratedWriteInstruction(IInstruction instruction) : base(instruction) {}
    }
    public abstract class WriteInstructionBase : InstructionBase<IWriting> {}

    public abstract class WriteInstructionBase<T> : WriteInstructionBase, IWriteInstruction<T>
    {
        protected override void Execute(IWriting services)
        {
            var instance = services.Current.Instance;
            if (instance is T)
            {
                Execute(services, (T) instance);
                return;
            }
            throw new InvalidOperationException(
                      $"Expected an instance of type '{typeof(T)}' but got an instance of '{instance.GetType()}'");
        }

        protected abstract void Execute(IWriting services, T instance);

        void IWriteInstruction<T>.Execute( IWriting services, T instance ) => Execute(services, instance);
    }

    
    class EmitDictionaryPairInstruction : WriteInstructionBase<DictionaryEntry>
    {
        private readonly IInstruction _key;
        private readonly IInstruction _value;

        public EmitDictionaryPairInstruction(IInstruction key, IInstruction value)
        {
            _key = key;
            _value = value;
        }

        protected override void Execute(IWriting services, DictionaryEntry instance)
        {
            using (services.New(instance.Key))
            {
                _key.Execute(services);
            }

            using (services.New(instance.Value))
            {
                _value.Execute(services);
            }
        }
    }

    class EmitDictionaryInstruction : WriteInstructionBase<IDictionary>
    {
        private readonly IInstruction _entry;

        public EmitDictionaryInstruction(IInstruction key, IInstruction value) : this(
            new EmitObjectInstruction(ExtendedXmlSerializer.Item,
                                           new EmitDictionaryPairInstruction(key, value))
        ) {}

        public EmitDictionaryInstruction(IInstruction entry)
        {
            _entry = entry;
        }

        protected override void Execute(IWriting services, IDictionary instance)
        {
            foreach (DictionaryEntry item in instance)
            {
                using (services.New(item))
                {
                    _entry.Execute(services);
                }
            }
        }
    }

    class EmitEnumerableInstruction : WriteInstructionBase<IEnumerable>
    {
        private readonly IInstruction _template;

        public EmitEnumerableInstruction(IInstruction template)
        {
            _template = template;
        }

        protected override void Execute(IWriting services, IEnumerable instance)
        {
            foreach (var item in Arrays.Default.AsArray(instance))
            {
                using (services.New(item))
                {
                    _template.Execute(services);
                }
            }
        }
    }

    /*class EmitCurrentInstanceTypeInstruction : EmitTypeInstruction
    {
        public static EmitCurrentInstanceTypeInstruction Default { get; } = new EmitCurrentInstanceTypeInstruction();
        EmitCurrentInstanceTypeInstruction() : base(context => context.Current.Instance.GetType()) {}
    }*/

    class EmitTypeInstruction : WriteInstructionBase
    {
        public static EmitTypeInstruction Default { get; } = new EmitTypeInstruction();
        EmitTypeInstruction() {}

        protected override void Execute(IWriting services)
        {
            var type = services.Current.Instance.GetType();
            var format = services.Get<ITypeFormatter>().Format(type);
            var property = new TypeProperty(format);
            services.Emit(property);
        }
    }

    class EmitMemberAsTextInstruction : WriteInstructionBase
    {
        public static EmitMemberAsTextInstruction Default { get; } = new EmitMemberAsTextInstruction();
        EmitMemberAsTextInstruction() {}

        protected override void Execute(IWriting services)
        {
            var member = services.Current.Member;
            if (member != null)
            {
                services.Emit(new MemberProperty(member.Value));
            }
            else
            {
                throw new InvalidOperationException($"An attempt was made to emit a member of '{services.Current.Instance.GetType()}' but it is not available.");
            }
        }
    }

    public interface INameProvider
    {
        string Get(IServiceProvider services);
    }

    class InstanceTypeNameProvider : INameProvider
    {
        public static InstanceTypeNameProvider Default { get; } = new InstanceTypeNameProvider();
        InstanceTypeNameProvider() {}

        public string Get(IServiceProvider services) => services.AsValid<IWritingContext>().Current.Instance.GetType().Name;
    }

    class TypeDefinitionNameProvider : FixedNameProvider
    {
        public TypeDefinitionNameProvider(Type type) : base(TypeDefinitionCache.GetDefinition(type).Name) {}
    }

    class FixedNameProvider : INameProvider
    {
        private readonly string _name;

        public FixedNameProvider(string name)
        {
            _name = name;
        }

        public string Get(IServiceProvider services) => _name;
    }

    class EmitObjectInstruction : DecoratedWriteInstruction
    {
        private readonly INameProvider _provider;

        public EmitObjectInstruction(string name, IInstruction instruction)
            : this(new FixedNameProvider(name), instruction) {}

        public EmitObjectInstruction(MemberInfo member, IInstruction instruction)
            : this(new MemberInfoNameProvider(member), instruction) {}

        public EmitObjectInstruction(Type type, IInstruction instruction)
            : this(new TypeDefinitionNameProvider(type), instruction) {}

        public EmitObjectInstruction(INameProvider provider, IInstruction instruction) : base(instruction)
        {
            _provider = provider;
        }

        protected override void Execute(IWriting services)
        {
            var name = _provider.Get(services);
            services.BeginContent(name);
            base.Execute(services);
            services.EndContent();
        }
    }

    abstract class EmitMembersInstructionBase : DecoratedWriteInstruction
    {
        private readonly Func<MemberContext, IMemberInstruction> _factory;
        
        protected EmitMembersInstructionBase(Func<MemberContext, IMemberInstruction> factory, IInstruction instruction) : base(instruction)
        {
            _factory = factory;
        }

        protected override void Execute(IWriting services)
        {
            var all = DetermineSet(services);

            var instructions = all.Select(_factory).ToArray();

            var properties = instructions.OfType<IPropertyInstruction>().ToArray();
            
            foreach (var instruction in properties)
            {
                instruction.Execute(services);
            }

            base.Execute(services);

            foreach (var instruction in instructions.Except(properties))
            {
                instruction.Execute(services);
            }
        }

        protected abstract IImmutableList<MemberContext> DetermineSet(IWriting services);
    }

    class EmitDifferentiatingMembersInstruction : EmitMembersInstructionBase
    {
        private readonly Func<Type, IImmutableList<MemberContext>> _differentiating;

        public EmitDifferentiatingMembersInstruction(Type type, Func<MemberContext, IMemberInstruction> factory,
                                                     IInstruction instruction)
            : this(DifferentiatingDefinitions.Default.Get(type), factory, instruction) {}

        public EmitDifferentiatingMembersInstruction(
            Func<Type, IImmutableList<MemberContext>> differentiating,
            Func<MemberContext, IMemberInstruction> factory,
            IInstruction instruction) : base(factory, instruction)
        {
            _differentiating = differentiating;
        }

        protected override IImmutableList<MemberContext> DetermineSet(IWriting services)
            => _differentiating(services.Current.Instance.GetType());
    }

    class EmitAttachedPropertiesInstruction : WriteInstructionBase
    {
        private readonly IWritePlan _primary;
        private readonly Func<object, bool> _specification;
        public EmitAttachedPropertiesInstruction(IWritePlan primary, Func<object, bool> specification)
        {
            _primary = primary;
            _specification = specification;
        }

        protected override void Execute(IWriting services)
        {
            var all = services.GetProperties();
            var properties = Properties(all).ToArray();
            foreach (var property in properties)
            {
                services.Emit(property);
            }

            foreach (var content in all.Except(properties))
            {
                new EmitObjectInstruction(content.Name, _primary.For(content.Value.GetType())).Execute(services);
            }
        }

        IEnumerable<IProperty> Properties(IEnumerable<IProperty> source)
        {
            foreach (var property in source)
            {
                if (_specification(property))
                {
                    yield return property;
                }
            }
        }
    }

    class EmitDeferredMembersInstruction : EmitMembersInstructionBase
    {
        private readonly IImmutableList<MemberInfo> _deferred;

        public EmitDeferredMembersInstruction(
            IImmutableList<MemberInfo> deferred,
            Func<MemberContext, IMemberInstruction> factory,
            IInstruction instruction) : base(factory, instruction)
        {
            _deferred = deferred;
        }

        protected override IImmutableList<MemberContext> DetermineSet(IWriting services) => Yield(services).ToImmutableList();

        private IEnumerable<MemberContext> Yield(IWritingContext context)
        {
            foreach (var member in _deferred)
            {
                yield return MemberContexts.Default.Locate(context.Current.Instance, member);
            }
        }
    }

    public interface IMemberInstruction : IInstruction {}
    public interface IPropertyInstruction : IMemberInstruction {}
    public interface IContentInstruction : IMemberInstruction {}

    class EmitGeneralObjectInstruction : WriteInstructionBase
    {
        private readonly IWritePlan _plan;
        public EmitGeneralObjectInstruction(IWritePlan plan)
        {
            _plan = plan;
        }

        protected override void Execute(IWriting services)
        {
            var type = services.Current.Instance.GetType();
            var selected = _plan.For(type);
            selected?.Execute(services);
        }
    }
    class ConditionalWriteInstruction : DecoratedWriteInstruction
    {
        private readonly ISpecification<IWritingContext> _specification;
        
        public ConditionalWriteInstruction(ISpecification<IWritingContext> specification, IInstruction instruction) : base(instruction)
        {
            _specification = specification;
        }

        protected override void Execute(IWriting services)
        {
            if (_specification.IsSatisfiedBy(services))
            {
                base.Execute(services);
            }
        }
    }


    class EmitTypeForTemplateInstruction : EmitWithTypeInstruction
    {
        readonly private static IInstruction Specification =
            new ConditionalWriteInstruction(EmitMemberTypeSpecification.Default, EmitTypeInstruction.Default);
        public EmitTypeForTemplateInstruction(IInstruction body) : base(Specification, body) {}
    }

    class EmitWithTypeInstruction : CompositeInstruction
    {
        public EmitWithTypeInstruction(IInstruction body) : this(EmitTypeInstruction.Default, body) {}
        
        public EmitWithTypeInstruction(IInstruction type, IInstruction body) : base(type, body) {}
    }

    abstract class NewWriteContextInstructionBase : NewContextInstructionBase<IWriting>
    {
        protected NewWriteContextInstructionBase(IInstruction instruction) : base(instruction) {}
    }

    class StartNewMembersContextInstruction : NewWriteContextInstructionBase
    {
        private readonly IImmutableList<MemberInfo> _members;
        public StartNewMembersContextInstruction(IImmutableList<MemberInfo> members, IInstruction instruction) : base(instruction)
        {
            _members = members;
        }
        protected override IDisposable DetermineContext(IWriting writing) => writing.New(_members);
    }

    class EmitInstanceAsTextInstruction : WriteInstructionBase
    {
        public static EmitInstanceAsTextInstruction Default { get; } = new EmitInstanceAsTextInstruction();
        EmitInstanceAsTextInstruction() {}

        protected override void Execute(IWriting services) => services.Emit(services.Current.Instance);
    }

    class EmitMemberAsPropertyInstruction : DecoratedWriteInstruction, IPropertyInstruction
    {
        public EmitMemberAsPropertyInstruction(IInstruction instruction) : base(instruction) {}
    }

    class EmitMemberAsContentInstruction : DecoratedWriteInstruction, IContentInstruction
    {
        public EmitMemberAsContentInstruction(IInstruction instruction) : base(instruction) {}
    }

    

    class StartNewMemberValueContextInstruction : NewWriteContextInstructionBase
    {
        public StartNewMemberValueContextInstruction(IInstruction content) : base(content) {}
        protected override IDisposable DetermineContext(IWriting writing) => writing.ToMemberContext();
    }

    class StartNewMemberContextInstruction : NewWriteContextInstructionBase
    {
        private readonly MemberInfo _member;

        public StartNewMemberContextInstruction(MemberInfo member, IInstruction instruction) : base(instruction)
        {
            _member = member;
        }

        protected override IDisposable DetermineContext(IWriting writing) => writing.New(_member);
    }

    class StartNewContextFromMemberValueInstruction : NewWriteContextInstructionBase
    {
        public StartNewContextFromMemberValueInstruction(IInstruction instruction) : base(instruction) {}
        protected override IDisposable DetermineContext(IWriting writing) => writing.New(writing.Current.Member?.Value);
    }

    class StartNewContextFromRootInstruction : NewWriteContextInstructionBase
    {
        public StartNewContextFromRootInstruction(IInstruction instruction) : base(instruction) {}

        protected override IDisposable DetermineContext(IWriting writing) => writing.New(writing.Current.Root);
    }

    /*class ExtensionEnabledInstruction : DecoratedWriteInstruction
    {
        private readonly IWritingExtension _extension;

        public ExtensionEnabledInstruction(IWritingExtension extension, IInstruction instruction) : base(instruction)
        {
            _extension = extension;
        }

        protected override void Execute(IWriting services)
        {
            base.Execute(services);
            /*if (_extension.Starting(writing))
            {
                base.Execute(writing);
            }
            _extension.Finished(writing);#1#
        }
    }*/

    /*class EnableExtensionInstructionAlteration : IAlteration<IInstruction>
    {
        private readonly ISpecification<IInstruction> _specification;
        private readonly IWritingExtension _extension;

        public EnableExtensionInstructionAlteration(IWritingExtension extension) : this(IsTypeSpecification<IContextInstruction>.Default, extension) {}

        public EnableExtensionInstructionAlteration(ISpecification<IInstruction> specification, IWritingExtension extension)
        {
            _specification = specification;
            _extension = extension;
        }

        public IInstruction Get(IInstruction parameter) => /*_specification.IsSatisfiedBy(parameter) ? new ExtensionEnabledInstruction(_extension, parameter) :#1# parameter;
    }*/
}