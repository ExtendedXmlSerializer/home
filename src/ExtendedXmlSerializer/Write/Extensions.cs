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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
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
        /*/// <summary>
        /// Attribution: http://stackoverflow.com/a/11221963/3602057
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="stream"></param>
        /// <param name="data"></param>
        /// <param name="namespaces"></param>
        public static void WriteObject(
            this XmlObjectSerializer serializer,
            Stream stream, object data,
            Dictionary<string, string> namespaces )
        {
            using ( var writer = XmlWriter.Create( stream, new XmlWriterSettings() ) )
            {
                serializer.WriteStartObject( writer, data );
                foreach ( var pair in namespaces )
                {
                    writer.WriteAttributeString( "xmlns", pair.Key, null, pair.Value );
                }
                serializer.WriteObjectContent( writer, data );
                serializer.WriteEndObject( writer );
            }
        }*/
        // public static TResult Accept<TParameter, TResult>( this TResult @this, TParameter _ ) => @this;

        /*public static void Property(this IWriting @this, IProperty property)
            => @this.Property(property.Name, @this.Serialize(property.Value));*/

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
        public DefaultWritingExtensions(ISerializationToolsFactory factory) : base(
            new ObjectReferencesExtension(factory),
            new VersionExtension(factory),
            new CustomSerializationExtension(factory)
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
        public static ObjectSerializer Default { get; } = new ObjectSerializer();
        ObjectSerializer() {}

        public string Serialize(object instance)
        {
            var result = instance as string ?? (instance as Enum)?.ToString() ?? PrimitiveValueTools.SetPrimitiveValue(instance);
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
        /*protected virtual bool StartingContent(IWriting services, object instance, MemberContext? context, string content) => true;*/

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
        /*protected virtual bool StartingContent(IWriting services, IExtendedXmlSerializerConfig configuration,
                                               object instance, MemberContext? member, string content) => true;*/
    }

    public class CustomSerializationExtension : ConfigurationWritingExtensionBase
    {
        private readonly IInstruction _instruction;

        public CustomSerializationExtension(ISerializationToolsFactory factory) : this(factory, EmitTypeInstruction.Default)
        {}

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

        public VersionExtension(ISerializationToolsFactory factory) : this(factory, EmitTypeInstruction.Default) {}

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
                services.Attach(new VersionProperty(version));
            }
            return base.StartingMembers(services, configuration, instance, members);
        }
    }

    class MemberProperty : PropertyBase, INamespace
    {
        public MemberProperty(MemberContext member) : base(member.Metadata.Name, member.Value) {}

        public Uri Identifier { get; } = null; // TODO: resolve from property value.
        public string Prefix => null; // TODO: resolve from property value.
    }

    class TypeProperty : PropertyBase
    {
        public TypeProperty(string type) : base(ExtendedXmlSerializer.Type, type) {}
    }

    class VersionProperty : PropertyBase
    {
        public VersionProperty(int version) : base(ExtendedXmlSerializer.Version, version) {}
    }

    class ObjectReferenceProperty : PropertyBase
    {
        public ObjectReferenceProperty(string value) : base(ExtendedXmlSerializer.Ref, value) {}
    }

    class ObjectIdProperty : PropertyBase
    {
        public ObjectIdProperty(string value) : base(ExtendedXmlSerializer.Id, value) {}
    }

    public class ObjectReferencesExtension : ConfigurationWritingExtensionBase
    {
        private readonly WeakCache<IWriting, Context> _contexts = new WeakCache<IWriting, Context>(_ => new Context());
        
        public ObjectReferencesExtension(ISerializationToolsFactory factory) : base(factory) {}

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
                var property = reference ? (IProperty) new ObjectReferenceProperty(objectId) : new ObjectIdProperty(objectId);
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
                        EmitTypeInstruction.Default.Execute(services);
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