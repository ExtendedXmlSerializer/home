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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Xml;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Common;

namespace ExtendedXmlSerialization.Write
{
    
    public static class Extensions
    {
        readonly private static int Length = Enum.GetNames(typeof(WriteState)).Length;

        public static WriteContext? Parent(this IWritingContext @this, int level = 1) => @this.Hierarchy.ElementAtOrDefault(level);

        public static WriteContext? GetArrayContext(this IWritingContext @this)
        {
            var parent = @this.Parent((int) @this.Current.State);
            var instance = parent?.Instance;
            var result = instance != null && Arrays.Default.Is(instance) ? parent : null;
            return result;
        }

        public static WriteContext? GetDictionaryContext(this IWritingContext @this)
        {
            var parent = @this.Parent((int) @this.Current.State+1);

            var instance = parent?.Instance;
            var result = instance != null && TypeDefinitionCache.GetDefinition(instance.GetType()).IsDictionary ? parent : null;
            return result;
        }

        public static WriteContext? GetMemberContext(this IWritingContext @this)
        {
            if (@this.Current.Member != null)
            {
                return @this.Current;
            }
            var parent = @this.Parent((int) @this.Current.State);
            var result = parent?.Member != null ? parent : null;
            return result;
        }
    }

    class DefaultWritingExtensions : CompositeExtension
    {
        public DefaultWritingExtensions(ISerializationToolsFactory factory, IInstruction instruction) : base(
            new ObjectReferencesExtension(factory, instruction),
            new VersionExtension(factory, instruction),
            new CustomSerializationExtension(factory, instruction)
        ) {}
    }

    class DefaultMemberValueAssignedExtension : MemberValueAssignedExtension
    {
        public new static DefaultMemberValueAssignedExtension Default { get; } = new DefaultMemberValueAssignedExtension();
        DefaultMemberValueAssignedExtension() {}

        protected override bool StartingMember(IWriting writing, object instance, MemberContext context) => 
            context.Value is Enum || base.StartingMember(writing, instance, context);
    }

    class MemberValueAssignedExtension : WritingExtensionBase
    {
        public static MemberValueAssignedExtension Default { get; } = new MemberValueAssignedExtension();
        protected MemberValueAssignedExtension() : this(DefaultValues.Default.Get) {}

        private readonly Func<Type, object> _values;

        public MemberValueAssignedExtension(Func<Type, object> values)
        {
            _values = values;
        }

        protected override bool StartingMember(IWriting services, object instance, MemberContext member)
        {
            var defaultValue = _values(member.MemberType);
            var result = !Equals(member.Value, defaultValue);
            return result;
        }
    }

    class ObjectSerializer : IObjectSerializer
    {
        private readonly ITypeFormatter _formatter;
        public static ObjectSerializer Default { get; } = new ObjectSerializer();
        ObjectSerializer() : this(DefaultTypeFormatter.Default) {}

        public ObjectSerializer(ITypeFormatter formatter)
        {
            _formatter = formatter;
        }

        public string Serialize(object instance)
        {
            var result = instance as string ?? (instance as Enum)?.ToString() ?? FromType(instance) ?? PrimitiveValueTools.SetPrimitiveValue(instance);
            return result;
        }

        private string FromType(object instance)
        {
            var type = instance as Type;
            var result = type != null ? _formatter.Format(type) : null;
            return result;
        }
    }

    public interface IWritingExtension : IExtension<IWriting> {}

    public abstract class WritingExtensionBase : ExtensionBase<IWriting>, IWritingExtension
    {
        public override bool Starting(IWriting services)
        {
            var current = services.Current;

            if (current.Member != null)
            {
                switch (current.State)
                {
                    case WriteState.MemberValue:
                        return StartingMemberValue(services, current.Instance, current.Member.GetValueOrDefault());
                    default:
                        return StartingMember(services, current.Instance, current.Member.GetValueOrDefault());
                }
            }

            if (current.Members != null)
            {
                return StartingMembers(services, current.Instance, current.Members);
            }

            var result = current.Instance != null
                ? StartingInstance(services, current.Instance)
                : Initializing(services);
            return result;
        }

        bool IExtension.Starting(IServiceProvider services) => Starting(services.AsValid<IWriting>());

        protected virtual bool Initializing(IWriting writing) => true;
        protected virtual bool StartingInstance(IWriting writing, object instance) => true;
        protected virtual bool StartingMembers(IWriting writing, object instance, IImmutableList<MemberInfo> members) => true;
        protected virtual bool StartingMember(IWriting writing, object instance, MemberContext context) => true;
        protected virtual bool StartingMemberValue(IWriting writing, object instance, MemberContext context) => true;

        void IExtension.Finished(IServiceProvider services) => Finished(services.AsValid<IWriting>());

        protected virtual void FinishedInstance(IWriting writing, object instance) {}
        protected virtual void FinishedMembers(IWriting writing, object instance, IImmutableList<MemberInfo> members) {}
        protected virtual void FinishedMember(IWriting writing, object instance, MemberContext member) {}
        protected virtual void FinishedMemberValue(IWriting writing, object instance, MemberContext member) {}
        protected virtual void Completed(IWriting writing) {}

        public override void Finished(IWriting services)
        {
            var current = services.Current;
            if (current.Member != null)
            {
                switch (current.State)
                {
                    case WriteState.MemberValue:
                        FinishedMemberValue(services, current.Instance, current.Member.GetValueOrDefault());
                        break;
                    default:
                        FinishedMember(services, current.Instance, current.Member.GetValueOrDefault());
                        break;
                }
                
            }
            else if (current.Members != null)
            {
                FinishedMembers(services, current.Instance, current.Members);
            }
            else if (current.Instance != null)
            {
                FinishedInstance(services, current.Instance);
            }
            else
            {
                Completed(services);
            }
        }
    }

    public abstract class ConfigurationWritingExtensionBase : WritingExtensionBase
    {
        private readonly ISerializationToolsFactory _factory;

        protected ConfigurationWritingExtensionBase(ISerializationToolsFactory factory)
        {
            _factory = factory;
        }

        protected IExtendedXmlSerializerConfig For(Type type) => _factory.GetConfiguration(type);

        public override bool Starting(IWriting writing)
        {
            var current = writing.Current;
            if (current.Instance != null)
            {
                var type = current.Instance.GetType();
                var configuration = For(type);
                if (configuration != null)
                {
                    if (current.Member != null)
                    {
                        switch (current.State)
                        {
                            case WriteState.MemberValue:
                                return StartingMemberValue(writing, configuration, current.Instance, current.Member.GetValueOrDefault());
                                
                            default:
                                return StartingMember(writing, configuration, current.Instance, current.Member.GetValueOrDefault());
                        }
                    }

                    var result = current.Members != null
                        ? StartingMembers(writing, configuration, current.Instance, current.Members)
                        : StartingInstance(writing, configuration, current.Instance);
                    return result;
                }
            }

            var starting = base.Starting(writing);
            return starting;
        }

        protected virtual bool StartingInstance(IWriting writing, IExtendedXmlSerializerConfig configuration,
                                                object instance) => true;
        protected virtual bool StartingMembers(IWriting writing, IExtendedXmlSerializerConfig configuration,
                                               object instance, IImmutableList<MemberInfo> members) => true;
        protected virtual bool StartingMember(IWriting writing, IExtendedXmlSerializerConfig configuration, object instance, MemberContext member) => true;
        protected virtual bool StartingMemberValue(IWriting writing, IExtendedXmlSerializerConfig configuration,
                                              object instance, MemberContext member) => true;
    }

    public class CustomSerializationExtension : ConfigurationWritingExtensionBase
    {
        private readonly IInstruction _instruction;

        public CustomSerializationExtension(ISerializationToolsFactory factory, IInstruction instruction) : base(factory)
        {
            _instruction = instruction;
        }

        protected override bool StartingMembers(IWriting services, IExtendedXmlSerializerConfig configuration, object instance,
                                                IImmutableList<MemberInfo> members)
        {
            if (configuration.IsCustomSerializer)
            {
                _instruction.Execute(services);
                configuration.WriteObject(services.Get<XmlWriter>(), instance);
                return false;
            }
            return base.StartingMembers(services, configuration, instance, members);    
        }
    }

    public class VersionExtension : ConfigurationWritingExtensionBase
    {
        private readonly IInstruction _instruction;

        public VersionExtension(ISerializationToolsFactory factory, IInstruction instruction) : base(factory)
        {
            _instruction = instruction;
        }

        protected override bool StartingMembers(IWriting services, IExtendedXmlSerializerConfig configuration, object instance,
                                                IImmutableList<MemberInfo> members)
        {
            var version = configuration.Version;
            if (version > 0)
            {
                _instruction.Execute(services);
                services.Attach(new VersionProperty(services.Get(this), version));
            }
            return base.StartingMembers(services, configuration, instance, members);
        }
    }

    class MemberProperty : PropertyBase
    {
        public MemberProperty(INamespace @namespace, MemberContext member) : base(@namespace, member.DisplayName, member.Value) {}
    }

    class TypeProperty : PropertyBase
    {
        public TypeProperty(INamespace @namespace, Type type) : base(@namespace, ExtendedXmlSerializer.Type, type) {}
    }

    class DictionaryItemElement : Element
    {
        public DictionaryItemElement(INamespace @namespace) : base(@namespace, ExtendedXmlSerializer.Item) {}
    }

    class DictionaryKeyElement : Element
    {
        public DictionaryKeyElement(INamespace @namespace) : base(@namespace, ExtendedXmlSerializer.Key) {}
    }

    class DictionaryValueElement : Element
    {
        public DictionaryValueElement(INamespace @namespace) : base(@namespace, ExtendedXmlSerializer.Value) {}
    }

    class VersionProperty : PropertyBase
    {
        public VersionProperty(INamespace @namespace, int version) : base(@namespace, ExtendedXmlSerializer.Version, version) {}
    }

    class ObjectReferenceProperty : PropertyBase
    {
        public ObjectReferenceProperty(INamespace @namespace, string value) : base(@namespace, ExtendedXmlSerializer.Ref, value) {}
    }

    class ObjectIdProperty : PropertyBase
    {
        public ObjectIdProperty(INamespace @namespace, string value) : base(@namespace, ExtendedXmlSerializer.Id, value) {}
    }

    public class ObjectReferencesExtension : ConfigurationWritingExtensionBase
    {
        private readonly WeakCache<IWriting, Context> _contexts = new WeakCache<IWriting, Context>(_ => new Context());
        private readonly IInstruction _instruction;

        public ObjectReferencesExtension(ISerializationToolsFactory factory, IInstruction instruction) : base(factory)
        {
            _instruction = instruction;
        }

        protected override bool StartingInstance(IWriting services, object instance)
        {
            var elements = _contexts.Get(services).Elements;
            foreach (var item in Arrays.Default.AsArray(instance))
            {
                if (!elements.Contains(item))
                {
                    elements.Add(item);
                }
            }
            return base.StartingInstance(services, instance);
        }

        protected override void FinishedInstance(IWriting writing, object instance)
        {
            if (Arrays.Default.Is(instance))
            {
                _contexts.Get(writing).Elements.Clear();
            }
            base.FinishedInstance(writing, instance);
        }

        protected override bool StartingMembers(IWriting services, IExtendedXmlSerializerConfig configuration, object instance,
                                                IImmutableList<MemberInfo> members)
        {
            if (configuration?.IsObjectReference ?? false)
            {
                var context = _contexts.Get(services);
                var elements = context.Elements;
                var references = context.References;
                var objectId = configuration.GetObjectId(instance);
                var contains = references.Contains(instance);
                var reference = contains || (services.GetArrayContext() == null && elements.Contains(instance));
                var @namespace = services.Get(this);
                var property = reference ? (IProperty) new ObjectReferenceProperty(@namespace, objectId) : new ObjectIdProperty(@namespace, objectId);
                var result = !reference;
                if (result)
                {
                    services.Attach(property);
                    references.Add(instance);
                }
                else
                {
                    // TODO: Find a more elegant way to handle this:
                    if (EmitMemberTypeSpecification.Default.IsSatisfiedBy(services))
                    {
                        _instruction.Execute(services);
                    }
                    services.Emit(property);
                }
                return result;
            }
            return true;
        }

        class Context
        {
            public Context() : this(new HashSet<object>(), new HashSet<object>()) {}

            public Context(ISet<object> references, ISet<object> elements)
            {
                References = references;
                Elements = elements;
            }

            public ISet<object> References { get; }
            public ISet<object> Elements { get; }
        }
    }
}