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
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.Processing;
using Xunit;

namespace ExtendedXmlSerialization.Test
{
    public class ContextsTests
    {
        [Fact]
        public void PrimitiveSerialization()
        {
            var builder = new StringBuilder();
            using (var writer = XmlWriter.Create(builder))
            {
                var serializer = new Serializer(IntegerWriter.Default);
                serializer.Serialize(writer, 6776);
            }

            var actual = builder.ToString();
            Assert.Equal(@"<?xml version=""1.0"" encoding=""utf-16""?><int>6776</int>", actual);
        }

        /*[Fact]
        public void PrimitiveDeserialization()
        {
            var data = @"<?xml version=""1.0"" encoding=""utf-16""?><int>6776</int>";
            var element = XElement.Parse(data);
            var root = new Root(new DocumentContentFactory(element).Get());
            var emitter = new InstanceEmitter();
            root.Execute(emitter);


            Assert.Equal(data, actual);
        }*/

        /*[Fact]
        public void InstanceSerialization()
        {
            var instance = new InstanceClass {PropertyName = "Hello World!"};
            var root = new Root(new ProvidedContentFactory(instance).Get());
            var builder = new StringBuilder();
            var writer = XmlWriter.Create(builder);
            var emitter = new DocumentEmitter(writer);
            root.Execute(emitter);
            writer.Flush();

            var actual = builder.ToString();
            Assert.Equal(
                @"<?xml version=""1.0"" encoding=""utf-16""?><InstanceClass><PropertyName>Hello World!</PropertyName></InstanceClass>",
                actual);
        }

        class InstanceClass
        {
            public string PropertyName { get; set; }
        }*/

        /*public interface ISource
        {
            object GetCurrentValue();
        }

        public interface IInstanceFactory : IParameterizedSource<XElement, object> {}*/

        public interface ITypeProvider : IParameterizedSource<XElement, Type> {}

        private class TypeProvider : ITypeProvider
        {
            public static TypeProvider Default { get; } = new TypeProvider();
            TypeProvider() : this(Types.Default) {}

            private readonly ITypeParser _parser;

            public TypeProvider(ITypeParser parser)
            {
                _parser = parser;
            }

            public Type Get(XElement parameter)
            {
                var value = parameter.Attribute(ExtendedXmlSerializer.Type)?.Value;
                var result = value != null ? _parser.Get(value) : null;
                if (result == null)
                {
                    throw new SerializationException($"Could not find TypeDefinition from provided value: {value}");
                }

                return result;
            }
        }

        /*class DocumentContentFactory : IInstanceFactory
        {
            private readonly ITypeProvider _provider;

            public DocumentContentFactory(ITypeProvider provider)
            {
                _provider = provider;
            }

            public object Get(XElement element)
            {
                var type = _provider.Get(element);
                if (Primitives.Default.ContainsKey(type))
                {
                    return ValueServices.Convert();
                }

                var members =
                    SelectMembers()
                        .Select(x => new Member(x.Name, new ProvidedContentFactory(x.GetValue(_source)).Get()));
                var instance = new Instance(TypeDefinitions.Default.Get(_instanceType).Name, members);
                return instance;
            }

            private ImmutableArray<IMemberDefinition> SelectMembers()
            {
                return TypeDefinitions.Default.Get(_instanceType)
                                      .Members;
            }

            public object Get(XElement parameter)
            {
                return null;
            }

            private static object GetPrimitiveValue(ITypeDefinition type, string value, string name)
            {
                try
                {
                    return PrimitiveValueTools.GetPrimitiveValue(value, type);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Unsuccessful conversion node {name} for type {type.Name} - value {value}", ex);
                }
            }
        }*/

        /*public class Primitives : Dictionary<Type, string>
        {
            public static Primitives Default { get; } = new Primitives();
            Primitives() : base(new Dictionary<Type, string>
                                {
                                    {typeof(bool), "boolean"},
                                    {typeof(char), "char"},
                                    {typeof(sbyte), "byte"},
                                    {typeof(byte), "unsignedByte"},
                                    {typeof(short), "short"},
                                    {typeof(ushort), "unsignedShort"},
                                    {typeof(int), "int"},
                                    {typeof(uint), "unsignedInt"},
                                    {typeof(long), "long"},
                                    {typeof(ulong), "unsignedLong"},
                                    {typeof(float), "float"},
                                    {typeof(double), "double"},
                                    {typeof(decimal), "decimal"},
                                    {typeof(DateTime), "dateTime"},
                                    {typeof(DateTimeOffset), "dateTimeOffset"},
                                    {typeof(string), "string"},
                                    {typeof(Guid), "guid"},
                                    {typeof(TimeSpan), "TimeSpan"},
                                }) {}
        }*/

        public interface ISerializer
        {
            void Serialize(XmlWriter writer, object instance);
        }

        private class Serializer : ISerializer
        {
            private readonly IConditionalWriter[] _writers;

            public Serializer(params IConditionalWriter[] writers)
            {
                _writers = writers;
            }

            public void Serialize(XmlWriter writer, object instance)
            {
                var type = instance.GetType();
                foreach (var definition in _writers)
                {
                    if (definition.IsSatisfiedBy(type))
                    {
                        definition.Write(writer, instance);
                        return;
                    }
                }
            }
        }

        /*public class IntegerDefinition : PrimitiveDefinition<int>
        {
            public static IntegerDefinition Default { get; } = new IntegerDefinition();
            IntegerDefinition() : this("int") {}

            public IntegerDefinition(string name) : base(XmlConvert.ToString, XmlConvert.ToInt32) {}
        }

        public class PrimitiveDefinition<T> : Definition<T>
        {
            public PrimitiveDefinition(Action<XmlWriter, T> serialize, Func<XElement, T> deserialize)
                : base(serialize, deserialize)
            {
                
            }
        }

        public class Definition<T> : DefinitionBase<T>
        {
            private readonly Action<XmlWriter, T> _serialize;
            private readonly Func<XElement,T> _deserialize;

            public Definition(Action<XmlWriter, T> serialize, Func<XElement,T> deserialize)
            {
                _serialize = serialize;
                _deserialize = deserialize;
            }

            protected override void OnSerialize(XmlWriter writer, T instance) => _serialize(writer, instance);

            protected override T OnDeserialize(XElement element) => _deserialize(element);
        }*/

        class IntegerWriter : PrimitiveWriterBase<int>
        {
            public static IntegerWriter Default { get; } = new IntegerWriter();
            IntegerWriter() : this("int") {}

            public IntegerWriter(string elementName) : base(elementName, XmlConvert.ToString) {}
        }

        abstract class PrimitiveWriterBase<T> : ConditionalWriter
        {
            protected PrimitiveWriterBase(string elementName, Func<T, string> serialize)
                : base(typeof(T), new ElementWriter(elementName, new ValueWriter<T>(serialize))) {}
        }

        public abstract class WriterBase<T> : WriterBase, IWriter<T>
        {
            public abstract void Write(XmlWriter writer, T instance);

            public sealed override void Write(XmlWriter writer, object instance) => Write(writer, (T) instance);
        }

        public abstract class WriterBase : IWriter
        {
            public abstract void Write(XmlWriter writer, object instance);
        }

        public interface IWriter<in T> : IWriter
        {
            void Write(XmlWriter writer, T instance);
        }

        public interface IConditionalWriter : ISpecification<Type>, IWriter {}

        class ConditionalWriter : DecoratedWriter, IConditionalWriter
        {
            private readonly Type _supportedType;

            public ConditionalWriter(Type supportedType, IWriter writer) : base(writer)
            {
                _supportedType = supportedType;
            }

            public bool IsSatisfiedBy(Type parameter) => _supportedType.IsAssignableFrom(parameter);
        }

        public interface IWriter
        {
            void Write(XmlWriter writer, object instance);
        }

        class ValueWriter : ValueWriter<object>
        {
            public ValueWriter(Func<object, string> serialize) : base(serialize) {}
        }

        class ValueWriter<T> : WriterBase<T>
        {
            private readonly Func<T, string> _serialize;

            public ValueWriter(Func<T, string> serialize)
            {
                _serialize = serialize;
            }

            public override void Write(XmlWriter writer, T instance) => writer.WriteString(_serialize(instance));
        }

        public class DecoratedWriter : WriterBase
        {
            private readonly IWriter _writer;

            public DecoratedWriter(IWriter writer)
            {
                _writer = writer;
            }

            public override void Write(XmlWriter writer, object instance) => _writer.Write(writer, instance);
        }

        class ElementWriter : DecoratedWriter
        {
            private readonly string _elementName;

            public ElementWriter(string elementName, IWriter writer) : base(writer)
            {
                _elementName = elementName;
            }

            public override void Write(XmlWriter writer, object instance)
            {
                writer.WriteStartElement(_elementName);
                base.Write(writer, instance);
                writer.WriteEndElement();
            }
        }

        /*class ElementWriter : ElementWriterBase
        {
            private readonly Action<XmlWriter, object> _write;

            public ElementWriter(string elementName, Action<XmlWriter, object> write) : base(elementName)
            {
                _write = write;
            }

            protected override void SerializeBody(XmlWriter writer, object instance) => _write(writer, instance);
        }

        abstract class ElementWriterBase : IWriter
        {
            private readonly string _elementName;

            protected ElementWriterBase(string elementName)
            {
                _elementName = elementName;
            }

            public void Write(XmlWriter writer, object instance)
            {
                writer.WriteStartElement(_elementName);
                SerializeBody(writer, instance);
                writer.WriteEndElement();
            }

            protected abstract void SerializeBody(XmlWriter writer, object instance);
        }*/


        /*public abstract class DefinitionBase<T> : DefinitionBase
                {
                    protected DefinitionBase() : base(typeof(T)) {}
        
                    protected abstract void OnSerialize(XmlWriter writer, T instance);
                    public sealed override void Serialize(XmlWriter writer, object instance) => OnSerialize(writer, (T)instance);
        
                    protected abstract T OnDeserialize(XElement element);
                    public override object Deserialize(XElement element) => OnDeserialize(element);
                }
        
                public abstract class DefinitionBase : IDefinition
                {
                    private readonly Type _supportedType;
                    protected DefinitionBase(Type supportedType)
                    {
                        _supportedType = supportedType;
                    }
        
                    public bool IsSatisfiedBy(Type parameter) => _supportedType.IsAssignableFrom(parameter);
        
                    // public abstract string Serialize(object instance);
        
                    public abstract void Serialize(XmlWriter writer, object instance);
                    public abstract object Deserialize(XElement element);
                }*/

        /*public interface IContentFactory : IParameterizedSource<object, IContext> {}

        public class ProvidedContentFactory : IContentFactory
        {
            /*private readonly object _source;
            private readonly Type _instanceType;

            public ProvidedContentFactory(object source) : this(source, source.GetType()) {}

            public ProvidedContentFactory(object source, Type instanceType)
            {
                _source = source;
                _instanceType = instanceType;
            }#1#

            public IContext Get(object parameter)
            {
                
                var type = parameter.GetType();
                if (Primitives.Default.ContainsKey(type))
                {
                    return new Primitive(Primitives.Default[type], parameter);
                }

                var definition = TypeDefinitions.Default.Get(type);
                var members =
                    definition.Members
                              .Select(x => new Member(x.Name, Get(x.GetValue(parameter))));
                var instance = new Instance(definition.Name, members);
                return instance;
            }
        }*/

        /*public interface IRoot : IContext {}

        class Root : DecoratedContext, IRoot
        {
            public Root(IContext context) : base(context) {}
        }*/

        /*public interface IEmitter
        {
            void Start(IContext context);
            void Emit(IContext context);
            void End(IContext context);
        }

        class DocumentEmitter : IEmitter
        {
            private readonly XmlWriter _writer;
            private readonly IObjectSerializer _serializer;

            public DocumentEmitter(XmlWriter writer) : this(writer, ObjectSerializer.Default) {}

            public DocumentEmitter(XmlWriter writer, IObjectSerializer serializer)
            {
                _writer = writer;
                _serializer = serializer;
            }

            public void Start(IContext context) => _writer.WriteStartElement(_serializer.Serialize(context));

            public void Emit(IContext context) => _writer.WriteString(_serializer.Serialize(context));

            public void End(IContext context) => _writer.WriteEndElement();
        }

        public class InstanceEmitter : ContextsTests.IEmitter
        {
            public void Start(IContext context) {}
            public void Emit(IContext context) {}
            public void End(IContext context) {}
        }*/

        /*public interface IContext //: ICommand<IEmitter>
        {
            string Name { get; }
        }

        public interface IInstance : IContext {}

        class Instance : ContextBase, IInstance
        {
            private readonly IEnumerable<IMember> _members;

            public Instance(string name, IEnumerable<IMember> members) : base(name)
            {
                _members = members;
            }

            public override void Execute(IEmitter parameter)
            {
                foreach (var member in _members)
                {
                    member.Execute(parameter);
                }
            }
        }*/

        /*public interface IMember : IContext
        {
            // bool IsWritable { get; }
        }

        class Member : DecoratedContext, IMember
        {
            public Member(string name, IContext context) : base(name, context) {}
        }

        public class DecoratedContext : ContextBase
        {
            private readonly IContext _context;

            public DecoratedContext(IContext context) : this(context.Name, context) {}

            public DecoratedContext(string name, IContext context) : base(name)
            {
                _context = context;
            }

            public override void Execute(IEmitter parameter)
            {
                parameter.Start(this);
                _context.Execute(parameter);
                parameter.End(this);
            }
        }*/

        /*public abstract class ContextBase : IContext, ISerializable
        {
            protected ContextBase(string name)
            {
                Name = name;
            }

            public string Name { get; }

            public abstract void Execute(IEmitter parameter);

            public virtual string Get(IObjectSerializer parameter) => Name;
        }*/

        /*public interface IPrimitive : IContext {}

        class Primitive : ContextBase, IPrimitive
        {
            private readonly object _instance;

            public Primitive(string name, object instance) : base(name)
            {
                _instance = instance;
            }

            public override void Execute(IEmitter parameter) => parameter.Emit(this);
            public override string Get(IObjectSerializer parameter) => parameter.Serialize(_instance);
        }*/
    }
}