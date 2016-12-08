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
using System.Xml.Serialization;
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
        protected override void OnExecute(IWriting services)
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

    class EmitDictionaryItemsInstruction : WriteInstructionBase<IDictionary>
    {
        private readonly IInstruction _template;

        public EmitDictionaryItemsInstruction(IInstruction template)
        {
            _template = template;
        }

        protected override void Execute(IWriting services, IDictionary instance)
        {
            foreach (DictionaryEntry item in instance)
            {
                using (New(services, item))
                {
                    _template.Execute(services);
                }
            }
        }

        private static IDisposable New<T>(IWritingContext services, T item) => services.New(item);
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

    class EmitTypeForInstanceInstruction : ConditionalInstruction<IWriting>
    {
        public static EmitTypeForInstanceInstruction Default { get; } = new EmitTypeForInstanceInstruction();
        EmitTypeForInstanceInstruction() : this(EmitTypeSpecification.Default) {}

        public EmitTypeForInstanceInstruction(ISpecification<IWriting> specification) : 
            base(specification, EmitTypeInstruction.Default) {}
    }

    class EmitTypeSpecification : ISpecification<IWritingContext>
    {
        public static EmitTypeSpecification Default { get; } = new EmitTypeSpecification();
        EmitTypeSpecification() {}

        public bool IsSatisfiedBy(IWritingContext parameter)
        {
            if (parameter.Current.Instance != parameter.Current.Root)
            {
                var context = parameter.GetMemberContext().GetValueOrDefault();
                switch (context.State)
                {
                    case WriteState.MemberValue:
                        var member = context.Member.GetValueOrDefault();
                        var result = member.IsWritable && member.Value.GetType() != member.MemberType;
                        return result;
                }
                /*var array = parameter.GetArrayContext();
                if (array != null)
                {
                    var elementType = ElementTypeLocator.Default.Locate(array.Value.Instance.GetType());
                    var result = parameter.Current.Instance.GetType() != elementType;
                    return result;
                }

                var dictionary = parameter.GetDictionaryContext();
                if (dictionary != null)
                {
                    var type = TypeDefinitionCache.GetDefinition(dictionary.Value.Instance.GetType()).GenericArguments[1];
                    var result = parameter.Current.Instance.GetType() != type;
                    return result;
                }*/
                var emit = parameter.GetArrayContext() == null && parameter.GetDictionaryContext() == null;
                return emit;
            }
            return false;
        }
    }

    class EmitTypeInstruction : WriteInstructionBase
    {
        public static EmitTypeInstruction Default { get; } = new EmitTypeInstruction();
        EmitTypeInstruction() {}

        protected override void OnExecute(IWriting services)
        {
            var type = services.Current.Instance.GetType();
            var property = new TypeProperty(services.Get(this), type);
            services.Emit(property);
        }
    }

    class EmitMemberAsTextInstruction : WriteInstructionBase
    {
        public static EmitMemberAsTextInstruction Default { get; } = new EmitMemberAsTextInstruction();
        EmitMemberAsTextInstruction() {}

        protected override void OnExecute(IWriting services)
        {
            var member = services.Current.Member;
            if (member != null)
            {
                var @namespace = member.Value.Metadata.DeclaringType.IsInstanceOfType(services.Current.Instance)
                    ? Namespace.Default.Identifier
                    : services.Get(member.Value.Metadata.DeclaringType);
                services.Emit(new MemberProperty(@namespace, member.Value));
            }
            else
            {
                throw new InvalidOperationException($"An attempt was made to emit a member of '{services.Current.Instance.GetType()}' but it is not available.");
            }
        }
    }

    public interface IElementProvider
    {
        IElement Get(INamespaceLocator locator, object instance);
    }

    class InstanceTypeNameProvider : ElementProviderBase
    {
        public static InstanceTypeNameProvider Default { get; } = new InstanceTypeNameProvider();
        InstanceTypeNameProvider() : base(o => TypeDefinitionCache.GetDefinition(o.GetType()).Name) {}
    }

    class TypeDefinitionElementProvider : FixedNameProvider
    {
        public TypeDefinitionElementProvider(Type type) : base(TypeDefinitionCache.GetDefinition(type).Name) {}
    }

    /*class FixedElementProvider : IElementProvider
    {
        private readonly IElement _element;
        public FixedElementProvider(IElement element)
        {
            _element = element;
        }

        public IElement Get(INamespaceLocator locator, object instance) => _element;
    }
*/
    class ApplicationElementProvider : DelegatedElementProvider
    {
        public ApplicationElementProvider(Func<Uri, object, IElement> element) : base(element) {}

        protected override Uri DetermineNamespace(INamespaceLocator locator, object instance) => locator.Get(GetType());
    }

    class DelegatedElementProvider : IElementProvider
    {
        private readonly Func<Uri, object, IElement> _element;

        public DelegatedElementProvider(Func<Uri, object, IElement> element)
        {
            _element = element;
        }

        public IElement Get(INamespaceLocator locator, object instance) => _element(DetermineNamespace(locator, instance), instance);

        protected virtual Uri DetermineNamespace(INamespaceLocator locator, object instance)
            => locator.Get(instance);
    }

    abstract class ElementProviderBase : IElementProvider
    {
        private readonly Func<object, string> _name;
        public ElementProviderBase(Func<object, string> name)
        {
            _name = name;
        }

        public IElement Get(INamespaceLocator locator, object instance)
        {
            var ns = locator.Get(instance);
            var element = new Element(ns, _name(instance));
            return element;
        }
    }

    class FixedNameProvider : ElementProviderBase
    {
        public FixedNameProvider(string name) : base(name.Accept) {}
    }

    class MemberInfoElementProvider : IElementProvider
    {
        private readonly MemberContext _member;

        public MemberInfoElementProvider(MemberContext member)
        {
            _member = member;
        }

        public IElement Get(INamespaceLocator locator, object instance) => new Element(locator.Get(_member.Metadata.DeclaringType), _member.DisplayName);
    }

    class EmitRootInstruction : DecoratedWriteInstruction
    {
        public EmitRootInstruction(IInstruction instruction) : base(instruction) {}

        protected override void OnExecute(IWriting services)
        {
            var root = services.Current.Root;
            var element = new RootElement(services.Get(root), root);
            services.Start(element);
            base.OnExecute(services);
            services.EndElement();
        }
    }
    class EmitInstanceInstruction : DecoratedWriteInstruction
    {
        private readonly IElementProvider _provider;

        public EmitInstanceInstruction(string name, IInstruction instruction)
            : this(new FixedNameProvider(name), instruction) {}

        public EmitInstanceInstruction(MemberContext member, IInstruction instruction)
            : this(new MemberInfoElementProvider(member), instruction) {}

        /*public EmitInstanceInstruction(Type type, IInstruction instruction)
            : this(new TypeDefinitionElementProvider(type), instruction) {}*/

        //public EmitInstanceInstruction(Func<INamespace, object, IElement> element, IInstruction instruction) : this(new DelegatedElementProvider(element), instruction) {}

        public EmitInstanceInstruction(IElementProvider provider, IInstruction instruction) : base(instruction)
        {
            _provider = provider;
        }

        protected override void OnExecute(IWriting services)
        {
            var element = _provider.Get(services, services.Current.Instance);
            services.Begin(element);
            base.OnExecute(services);
            services.EndElement();
        }
    }

    abstract class EmitMembersInstructionBase : DecoratedWriteInstruction
    {
        private readonly Func<MemberContext, IMemberInstruction> _factory;
        
        protected EmitMembersInstructionBase(Func<MemberContext, IMemberInstruction> factory, IInstruction instruction) : base(instruction)
        {
            _factory = factory;
        }

        protected override void OnExecute(IWriting services)
        {
            var all = DetermineSet(services);

            var instructions = all.Select(_factory).ToArray();

            var properties = instructions.OfType<IPropertyInstruction>().ToArray();
            
            foreach (var instruction in properties)
            {
                instruction.Execute(services);
            }

            base.OnExecute(services);

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
        private readonly IPlan _primary;
        private readonly Func<object, bool> _specification;
        public EmitAttachedPropertiesInstruction(IPlan primary, Func<object, bool> specification)
        {
            _primary = primary;
            _specification = specification;
        }

        protected override void OnExecute(IWriting services)
        {
            var all = services.GetProperties();
            var properties = Properties(all).ToArray();
            foreach (var property in properties)
            {
                services.Emit(property);
            }

            foreach (var content in all.Except(properties))
            {
                new EmitInstanceInstruction(content.Name, _primary.For(content.Value.GetType())).Execute(services);
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
        private readonly IPlan _plan;
        public EmitGeneralObjectInstruction(IPlan plan)
        {
            _plan = plan;
        }

        protected override void OnExecute(IWriting services)
        {
            var type = services.Current.Instance.GetType();
            var selected = _plan.For(type);
            selected?.Execute(services);
        }
    }
    

    /*class EmitTypeForTemplateInstruction : CompositeInstruction
    {
        public EmitTypeForTemplateInstruction(IInstruction emitType, params IInstruction[] body)
            : base(new ConditionalInstruction<IWriting>(EmitMemberTypeSpecification.Default, emitType).Fixed(body)) {}
    }*/

    /*class EmitWithTypeInstruction : CompositeInstruction
    {
        //public EmitWithTypeInstruction(IInstruction body) : this(EmitTypeInstruction.Default, body) {}
        
        public EmitWithTypeInstruction(IInstruction type, IInstruction body) : base(type, body) {}
    }*/

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

        protected override void OnExecute(IWriting services) => services.Emit(services.Current.Instance);
    }

    class EmitMemberAsPropertyInstruction : DecoratedInstruction, IPropertyInstruction
    {
        public EmitMemberAsPropertyInstruction(IInstruction instruction) : base(instruction) {}
    }

    class EmitMemberAsContentInstruction : DecoratedInstruction, IContentInstruction
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