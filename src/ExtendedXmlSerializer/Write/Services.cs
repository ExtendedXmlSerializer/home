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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Common;

namespace ExtendedXmlSerialization.Write
{
    public interface ISerializer
    {
        void Serialize(Stream stream, object instance);
    }

    public class Serializer : ISerializer
    {
        private readonly IPlan _plan;
        private readonly IWritingFactory _factory;

        public Serializer(IPlan plan, IWritingFactory factory)
        {
            _plan = plan;
            _factory = factory;
        }

        public void Serialize(Stream stream, object instance)
        {
            using (var writing = _factory.Get(stream))
            {
                using (writing.Start(instance))
                {
                    var instruction = _plan.For(instance.GetType());
                    instruction.Execute(writing);
                }
            }
        }
    }

    public interface IWriter : IDisposable
    {
        void Start(IRootElement root);

        void Begin(IElement element);

        void EndElement();

        void Emit(object instance);

        void Emit(IProperty property);
    }

    class Writer : IWriter
    {
        private readonly IObjectSerializer _serializer;
        private readonly INamespaceLocator _locator;
        private readonly INamespaceEmitter _emitter;
        private readonly XmlWriter _writer;

        public Writer(IObjectSerializer serializer, INamespaceLocator locator, INamespaceEmitter emitter, XmlWriter writer)
        {
            _serializer = serializer;
            _locator = locator;
            _emitter = emitter;
            _writer = writer;
        }

        public void Start(IRootElement root)
        {
            _writer.WriteStartDocument();
            Begin(root);
            var identifier = root.Identifier?.ToString();
            {
                if (identifier != null)
                {
                    _writer.WriteAttributeString("xmlns", identifier);
                    _emitter.Execute(root.Root);
                }
            }
        }
        public void Begin(IElement element) => _writer.WriteStartElement(element.Name, element.Identifier?.ToString());

        public void EndElement() => _writer.WriteEndElement();
        public void Emit(object instance) => _writer.WriteString(_serializer.Serialize(instance));

        public void Emit(IProperty property)
        {
            var ns = property.Identifier?.ToString();
            var type = property.Value as Type;
            if (ns != null && type != null)
            {
                var identifier = _locator.Get(property.Value)?.Identifier?.ToString();
                if (identifier != null)
                {
                    var name = TypeDefinitionCache.GetDefinition(type).Name;
                    _writer.WriteStartAttribute(property.Name, ns);
                    _writer.WriteQualifiedName(name, identifier);
                    _writer.WriteEndAttribute();
                    return;
                }
            }
            _writer.WriteAttributeString(property.Name, ns, _serializer.Serialize(property.Value));
        }

        public void Dispose()
        {
            _writer.WriteEndDocument();
            _writer.Dispose();
        }
    }

    public interface IWriting : IWriter, IWritingContext, INamespaceLocator, IServiceProvider
    {
        void Attach(IProperty property);
        IImmutableList<IProperty> GetProperties();
    }

    public enum WriteState { Root, Instance, Members, Member, MemberValue }

    public class MemberContexts : WeakCacheBase<object, IImmutableList<MemberContext>>
    {
        public static MemberContexts Default { get; } = new MemberContexts();
        MemberContexts() {}

        protected override IImmutableList<MemberContext> Callback(object key) => Yield(key).ToImmutableList();

        public MemberContext Locate(object instance, MemberInfo member)
        {
            foreach (var memberContext in Get(instance))
            {
                if (MemberInfoEqualityComparer.Default.Equals(memberContext.Metadata, member))
                {
                    return memberContext;
                }
            }
            throw new InvalidOperationException($"Could not find the member '{member}' for instance of type '{instance.GetType()}'");
        }

        static IEnumerable<MemberContext> Yield(object key)
        {
            var members = SerializableMembers.Default.Get(key.GetType());
            foreach (var member in members)
            {
                var getter = Getters.Default.Get(member);
                yield return new MemberContext(member, getter(key));
            }
        }
    }

    public struct MemberContext
    {
        public MemberContext(MemberInfo member, object value = null) : this(member, MemberNames.Default.Get(member), member.GetMemberType(), member.IsWritable(), value) {}

        public MemberContext(MemberInfo metadata, string displayName, Type memberType, bool isWritable, object value)
        {
            Metadata = metadata;
            DisplayName = displayName;
            MemberType = memberType;
            IsWritable = isWritable;
            Value = value;
        }

        public MemberInfo Metadata { get; }
        public string DisplayName { get; }
        public Type MemberType { get; }
        public bool IsWritable { get; }
        public object Value { get; }
    }

    public struct WriteContext
    {
        public WriteContext(WriteState state, object root, object instance, IImmutableList<MemberInfo> members,
                            MemberContext? member)
        {
            State = state;
            Root = root;
            Instance = instance;
            Members = members;
            Member = member;
        }

        public WriteState State { get; }
        public object Root { get; }
        public object Instance { get; }
        public IImmutableList<MemberInfo> Members { get; }
        public MemberContext? Member { get; }
    }

    public interface IProperty : IElement
    {
        object Value { get; }
    }

    public class Element : Namespace, IElement
    {
        public Element(INamespace @namespace, string name) : base(@namespace?.Prefix, @namespace?.Identifier)
        {
            Name = name;
        }

        public string Name { get; }
    }

    abstract class PropertyBase : Element, IProperty
    {
        protected PropertyBase(INamespace @namespace, string name, object value) : base(@namespace, name)
        {
            Value = value;
        }

        public object Value { get; }
    }

    public interface IWritingContext
    {
        WriteContext Current { get; }
        IEnumerable<WriteContext> Hierarchy { get; }

        IDisposable Start(object root);
        IDisposable New(object instance);
        IDisposable New(IImmutableList<MemberInfo> members);
        IDisposable New(MemberInfo member);
        IDisposable ToMemberContext();
    }

    class DefaultWritingContext : IWritingContext
    {
        private readonly IAlteration<WriteContext> _alteration;
        readonly private Stack<WriteContext> _chain = new Stack<WriteContext>();
        readonly private DelegatedDisposable _popper;

        public DefaultWritingContext() : this(Self<WriteContext>.Default) {}

        public DefaultWritingContext(IAlteration<WriteContext> alteration)
        {
            _alteration = alteration;
            _popper = new DelegatedDisposable(Undo);
        }

        public WriteContext Current => _chain.FirstOrDefault();

        IDisposable New(WriteContext context)
        {
            _chain.Push(_alteration.Get(context));
            return _popper;
        }
        void Undo() => _chain.Pop();

        public IDisposable Start(object root)
        {
            if (_chain.Any())
            {
                throw new InvalidOperationException("A request to start a new writing context was made, but it has already started.");
            }
            return New(new WriteContext(WriteState.Root, root, null, null, null));
        }

        public IDisposable New(object instance)
        {
            var previous = _chain.Peek();
            var result = New(new WriteContext(WriteState.Instance, previous.Root, instance, null, null));
            return result;
        }

        public IDisposable New(IImmutableList<MemberInfo> members)
        {
            var previous = _chain.Peek();
            var result = New(new WriteContext(WriteState.Members, previous.Root, previous.Instance, members, null));
            return result;
        }
        
        public IDisposable New(MemberInfo member)
        {
            var previous = _chain.Peek();
            var found = MemberContexts.Default.Locate(previous.Instance, member);
            var context = new WriteContext(WriteState.Member, previous.Root, previous.Instance, previous.Members,
                                           found);
            var result = New(context);
            return result;
        }

        public IDisposable ToMemberContext()
        {
            var previous = _chain.Peek();
            var context = new WriteContext(WriteState.MemberValue, previous.Root, previous.Instance, previous.Members, previous.Member);
            var result = New(context);
            return result;
        }
        
        public IEnumerable<WriteContext> Hierarchy
        {
            get
            {
                foreach (var context in _chain)
                {
                    yield return context;
                }
            }
        }
    }

    public interface IAttachedProperties
    {
        void Attach(object instance, IProperty property);
        ICollection<IProperty> GetProperties(object instance);
    }

    class AttachedProperties : IAttachedProperties
    {
        public static AttachedProperties Default { get; } = new AttachedProperties();
        AttachedProperties() {}

        private readonly WeakCache<object, ICollection<IProperty>> 
            _properties = new WeakCache<object, ICollection<IProperty>>(_ => new Collection<IProperty>());

        public void Attach(object instance, IProperty property) => _properties.Get(instance).Add(property);
        public ICollection<IProperty> GetProperties(object instance) => _properties.Get(instance);
    }

    public interface IWritingFactory : IParameterizedSource<Stream, IWriting> {}

    public class WritingFactory : IWritingFactory
    {
        private readonly ISerializationToolsFactory _tools;
        private readonly INamespaces _namespaces;
        private readonly INamespaceLocator _locator;
        private readonly IServiceProvider _services;
        private readonly IExtension _extension;
        private readonly Func<IWritingContext> _context;

        public WritingFactory(
            ISerializationToolsFactory tools,
            INamespaces namespaces,
            INamespaceLocator locator,
            IServiceProvider services,
            Func<IWritingContext> context, IExtension extension)
        {
            _tools = tools;
            _namespaces = namespaces;
            _locator = locator;
            _services = services;
            _extension = extension;
            _context = context;
        }

        public IWriting Get(Stream parameter)
        {
            var context = _context();
            var settings = new XmlWriterSettings {NamespaceHandling = NamespaceHandling.OmitDuplicates, Indent = true};
            var xmlWriter = XmlWriter.Create(parameter, settings);
            var serializer = new EncryptedObjectSerializer(new EncryptionSpecification(_tools, context), _tools);
            var writer = new Writer(serializer, _locator, new NamespaceEmitter(xmlWriter, _namespaces), xmlWriter);
            var result = new Writing(writer, context, _locator
                                     /*services:*/, serializer, _extension, _tools, _services, this, parameter, context,
                                     settings, xmlWriter, serializer, writer);
            return result;
        }
    }

    public interface INamespaceEmitter
    {
        void Execute(object instance);
    }

    public interface INamespaces : IParameterizedSource<object, IImmutableList<INamespace>> {}

    public class Namespaces : INamespaces
    {
        private readonly INamespaceLocator _locator;
        private readonly INamespace[] _namespaces;

        public Namespaces(INamespaceLocator locator, params INamespace[] namespaces)
        {
            _locator = locator;
            _namespaces = namespaces;
        }

        public IImmutableList<INamespace> Get(object parameter) => _namespaces.Concat(Yield(parameter)).Distinct().OrderBy(x => x.Prefix).ToImmutableList();

        private IEnumerable<INamespace> Yield(object parameter) =>
            new NamespaceWalker(parameter, _locator).SelectMany(locations => locations);
    }

    class NamespaceWalker : ObjectWalkerBase<object, IEnumerable<INamespace>>
    {
        private readonly INamespaceLocator _locator;
        private readonly ISpecification<Type> _primitive;

        public NamespaceWalker(object root, INamespaceLocator locator)
            : this(root, locator, IsPrimitiveSpecification.Default) {}

        public NamespaceWalker(object root, INamespaceLocator locator, ISpecification<Type> primitive) : base(root)
        {
            _locator = locator;
            _primitive = primitive;
        }

        protected override IEnumerable<INamespace> Select(object input)
        {
            var type = input as Type;
            if (type != null)
            {
                yield return _locator.Get(type);
                
                foreach (var info in SerializableMembers.Default.Get(type))
                {
                    var memberType = info.GetMemberType();
                    if (info.IsWritable() && !_primitive.IsSatisfiedBy(memberType))
                    {
                        Schedule(memberType);
                    }

                    var definition = TypeDefinitionCache.GetDefinition(memberType);
                    if (definition.IsDictionary)
                    {
                        foreach (var argument in definition.GenericArguments)
                        {
                            if (!_primitive.IsSatisfiedBy(argument))
                            {
                                Schedule(argument);
                            }
                        }
                    }
                    else
                    {
                        var elementType = ElementTypeLocator.Default.Locate(memberType);
                        if (elementType != null)
                        {
                            Schedule(elementType);
                        }
                    }
                }
            }
            else
            {
                var dictionary = input as IDictionary;
                if (dictionary != null)
                {
                    var arguments = TypeDefinitionCache.GetDefinition(dictionary.GetType()).GenericArguments;
                    foreach (DictionaryEntry entry in dictionary)
                    {
                        var key = entry.Key.GetType();
                        if (key != arguments[0])
                        {
                            Schedule(key);
                            Schedule(entry.Key);
                        }

                        var value = entry.Value.GetType();
                        if (value != arguments[1])
                        {
                            Schedule(value);
                            Schedule(entry.Value);
                        }
                    }
                }
                else if (Arrays.Default.Is(input))
                {
                    var inputType = input.GetType();
                    var elementType = ElementTypeLocator.Default.Locate(inputType);

                    Schedule(elementType);
                    foreach (var element in Arrays.Default.AsArray(input))
                    {
                        var instanceType = element.GetType();
                        if (instanceType != elementType)
                        {
                            // Schedule(instanceType);
                            Schedule(element);
                        }
                    }
                }
                else
                {
                    foreach (var context in MemberContexts.Default.Get(input))
                    {
                        if (context.IsWritable)
                        {
                            if (!_primitive.IsSatisfiedBy(context.MemberType))
                            {
                                Schedule(context.MemberType);
                            }
                            if (context.Value != DefaultValues.Default.Get(context.MemberType))
                            {
                                var instanceType = context.Value.GetType();
                                if (instanceType != context.MemberType)
                                {
                                    Schedule(instanceType);
                                    Schedule(context.Value);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    class NamespaceEmitter : INamespaceEmitter
    {
        private const string Prefix = "xmlns";
        private readonly XmlWriter _writer;
        private readonly IParameterizedSource<object, IImmutableList<INamespace>> _namespaces;

        public NamespaceEmitter(XmlWriter writer, IParameterizedSource<object, IImmutableList<INamespace>> namespaces)
        {
            _writer = writer;
            _namespaces = namespaces;
        }

        public void Execute(object instance)
        {
            foreach (var pair in _namespaces.Get(instance))
            {
                _writer.WriteAttributeString(Prefix, pair.Prefix ?? string.Empty, null, pair.Identifier?.ToString());
            }
        }
    }

    class Writing : IWriting
    {
        private readonly IWriter _writer;
        private readonly IAttachedProperties _properties;
        private readonly INamespaceLocator _locator;
        private readonly IWritingContext _context;
        private readonly IServiceProvider _services;

        public Writing(IWriter writer, IWritingContext context, INamespaceLocator locator, params object[] services)
            : this(writer, context, AttachedProperties.Default, locator, new CompositeServiceProvider(services)) {}

        public Writing(IWriter writer, IWritingContext context, IAttachedProperties properties, INamespaceLocator locator, IServiceProvider services)
        {
            _writer = writer;
            _context = context;
            _properties = properties;
            _locator = locator;
            _services = services;
        }

        public object GetService(Type serviceType) => serviceType.GetTypeInfo().IsInstanceOfType(this) ? this : _services.GetService(serviceType);

        public void Start(IRootElement root) => _writer.Start(root);
        public void Begin(IElement element) => _writer.Begin(element);
        public void EndElement() => _writer.EndElement();
        public void Emit(object instance) => _writer.Emit(instance);
        public void Emit(IProperty property) => _writer.Emit(property);

        public void Dispose() => _writer.Dispose();

        public void Attach(IProperty property) => _properties.Attach(_context.Current.Instance, property);

        public IImmutableList<IProperty> GetProperties()
        {
            var list = _properties.GetProperties(_context.Current.Instance);
            var result = list.ToImmutableList();
            list.Clear();
            return result;
        }

        public IDisposable Start(object root) => _context.Start(root);
        public IDisposable New(object instance) => _context.New(instance);
        public IDisposable New(IImmutableList<MemberInfo> members) => _context.New(members);
        public IDisposable New(MemberInfo member) => _context.New(member);
        public IDisposable ToMemberContext() => _context.ToMemberContext();

        public WriteContext Current => _context.Current;
        public IEnumerable<WriteContext> Hierarchy => _context.Hierarchy;
        public INamespace Get(object parameter) => _locator.Get(parameter);
    }

    public interface IObjectSerializer
    {
        string Serialize(object instance);
    }

    class EncryptionSpecification : ISpecification<object>
    {
        private readonly ISerializationToolsFactory _factory;
        private readonly IWritingContext _context;
        public EncryptionSpecification(ISerializationToolsFactory factory, IWritingContext context)
        {
            _factory = factory;
            _context = context;
        }

        public bool IsSatisfiedBy(object parameter)
        {
            var context = _context.GetMemberContext();
            if (context != null)
            {
                var configuration = _factory.GetConfiguration(context?.Instance.GetType());
                if (configuration != null)
                {
                    var member = context?.Member?.Metadata;
                    if (member != null)
                    {
                        var result =
                            configuration.CheckPropertyEncryption(member.Name);
                        return result;
                    }
                }
            }
            return false;
        }
    }

    class EncryptedObjectSerializer : IObjectSerializer
    {
        private readonly ISpecification<object> _specification;
        private readonly ISerializationToolsFactory _factory;
        private readonly IObjectSerializer _inner;
        

        public EncryptedObjectSerializer(ISpecification<object> specification, ISerializationToolsFactory factory)
            : this(specification, factory, ObjectSerializer.Default)
        {
            _specification = specification;
        }

        public EncryptedObjectSerializer(ISpecification<object> specification, ISerializationToolsFactory factory, IObjectSerializer inner)
        {
            _specification = specification;
            _factory = factory;
            _inner = inner;
        }

        public string Serialize(object instance)
        {
            var text = _inner.Serialize(instance);
            var algorithm = _factory.EncryptionAlgorithm;
            if (algorithm != null && _specification.IsSatisfiedBy(instance))
            {
                var result = algorithm.Encrypt(text);
                return result;
            }
            return text;
        }
    }
}