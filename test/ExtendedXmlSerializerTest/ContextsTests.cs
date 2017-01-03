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
using System.IO;
using System.Linq;
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
        public void PrimitiveWrite()
        {
            var stream = new MemoryStream();
            var serializer = new Serializer(new ConditionalCompositeWriter(IntegerConverter.Default));
            serializer.Serialize(stream, 6776);
            stream.Seek(0, SeekOrigin.Begin);
            var actual = new StreamReader(stream).ReadToEnd();
            Assert.Equal(@"<?xml version=""1.0"" encoding=""utf-8""?><int>6776</int>", actual);
        }

        [Fact]
        public void PrimitiveRead()
        {
            const string data = @"<?xml version=""1.0"" encoding=""utf-8""?><int>6776</int>";
            var deserializer = new Deserializer(new ConditionalCompositeReader(new HintedRootTypeProvider(typeof(int), TypeProvider.Default), IntegerConverter.Default));
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
            var actual = deserializer.Deserialize(stream);
            Assert.Equal(6776, actual);
        }

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

        interface ITypeProvider : IParameterizedSource<XElement, Type> {}

        class HintedRootTypeProvider : ITypeProvider
        {
            private readonly Type _hint;
            private readonly ITypeProvider _provider;

            public HintedRootTypeProvider(Type hint, ITypeProvider provider)
            {
                _hint = hint;
                _provider = provider;
            }

            public Type Get(XElement parameter) => parameter.Document.Root == parameter ? _hint : _provider.Get(parameter);
        }

        class TypeProvider : ITypeProvider
        {
            public static TypeProvider Default { get; } = new TypeProvider();
            TypeProvider() : this(Types.Default, Identities.Default) {}

            private readonly ITypeParser _parser;
            private readonly IIdentities _identities;

            public TypeProvider(ITypeParser parser, IIdentities identities)
            {
                _parser = parser;
                _identities = identities;
            }

            public Type Get(XElement parameter) => 
                _identities.Get(parameter.Name) ?? FromAttribute(parameter);

            private Type FromAttribute(XElement parameter)
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

        interface IConditionalReader : ISpecification<Type>, IReader {}

        interface IReader
        {
            object Read(XElement element);
        }

        abstract class ReaderBase : IReader
        {
            public abstract object Read(XElement element);
        }

        abstract class ReaderBase<T> : IReader
        {
            object IReader.Read(XElement element) => Read(element);

            protected abstract T Read(XElement element);
        }

        class ValueReader<T> : ReaderBase<T>
        {
            private readonly Func<string, T> _deserialize;

            public ValueReader(Func<string, T> deserialize)
            {
                _deserialize = deserialize;
            }

            protected override T Read(XElement element) => _deserialize(element.Value);
        }

        class DecoratedReader : ReaderBase
        {
            private readonly IReader _reader;

            public DecoratedReader(IReader reader)
            {
                _reader = reader;
            }

            public override object Read(XElement element) => _reader.Read(element);
        }

        class ConditionalCompositeReader : IReader
        {
            private readonly IConditionalReader[] _readers;
            private readonly ITypeProvider _provider;


            public ConditionalCompositeReader(params IConditionalReader[] readers) : this(TypeProvider.Default, readers) {}

            public ConditionalCompositeReader(ITypeProvider provider, params IConditionalReader[] readers)
            {
                _provider = provider;
                _readers = readers;
            }

            public object Read(XElement element)
            {
                var type = _provider.Get(element);
                foreach (var reader in _readers)
                {
                    if (reader.IsSatisfiedBy(type))
                    {
                        var result = reader.Read(element);
                        return result;
                    }
                }
                return null;
            }
        }

        interface IDeserializer
        {
            object Deserialize(Stream stream);
        }

        class Deserializer : IDeserializer
        {
            private readonly IReader _reader;

            public Deserializer(IReader reader)
            {
                _reader = reader;
            }

            public object Deserialize(Stream stream)
            {
                var text = new StreamReader(stream).ReadToEnd();
                var element = XDocument.Parse(text).Root;
                var result = _reader.Read(element);
                return result;
            }
        }

        interface ISerializer
        {
            void Serialize(Stream stream, object instance);
        }

        class ConditionalCompositeWriter : IWriter
        {
            private readonly IConditionalWriter[] _writers;

            public ConditionalCompositeWriter(params IConditionalWriter[] writers)
            {
                _writers = writers;
            }

            public void Write(XmlWriter writer, object instance)
            {
                var type = instance.GetType();
                foreach (var item in _writers)
                {
                    if (item.IsSatisfiedBy(type))
                    {
                        item.Write(writer, instance);
                        return;
                    }
                }
            }
        }

        class Serializer : ISerializer
        {
            private readonly IWriter _writer;

            public Serializer(IWriter writer)
            {
                _writer = writer;
            }

            public void Serialize(Stream stream, object instance)
            {
                using (var writer = XmlWriter.Create(stream))
                {
                    _writer.Write(writer, instance);
                }
            }
        }

        interface IIdentities : ISpecification<Type>, ISpecification<XName>,
                                             IParameterizedSource<Type, XName>, IParameterizedSource<XName, Type> {}

        class Identities : IIdentities
        {
            public static Identities Default { get; } = new Identities();
            Identities() : this(string.Empty) {}

            private readonly IDictionary<Type, XName> _names;
            private readonly IDictionary<XName, Type> _types;

            public Identities(string namespaceName)
                : this(new Dictionary<Type, XName>
                       {
                           {typeof(int), XName.Get("int", namespaceName)}
                       }) {}

            public Identities(IDictionary<Type, XName> names)
                : this(names, names.ToDictionary(x => x.Value, y => y.Key)) {}

            public Identities(IDictionary<Type, XName> names, IDictionary<XName, Type> types)
            {
                _names = names;
                _types = types;
            }


            public XName Get(Type parameter)
            {
                XName result;
                return _names.TryGetValue(parameter, out result) ? result : null;
            }

            public Type Get(XName parameter)
            {
                Type result;
                return _types.TryGetValue(parameter, out result) ? result : null;
            }

            public bool IsSatisfiedBy(Type parameter) => _names.ContainsKey(parameter);

            public bool IsSatisfiedBy(XName parameter) => _types.ContainsKey(parameter);
        }

        interface IConverter : IConditionalWriter, IConditionalReader {}

        class Converter<T> : ConverterBase<T>
        {
            private readonly ISpecification<Type> _specification;
            private readonly IWriter _writer;
            private readonly IReader _reader;

            public Converter(ISpecification<Type> specification, IWriter writer, IReader reader)
            {
                _specification = specification;
                _writer = writer;
                _reader = reader;
            }

            public override bool IsSatisfiedBy(Type parameter) => _specification.IsSatisfiedBy(parameter);
            public override void Write(XmlWriter writer, object instance) => _writer.Write(writer, instance);
            public override object Read(XElement element) => _reader.Read(element);
        }

        abstract class ConverterBase<T> : IConverter
        {
            public abstract bool IsSatisfiedBy(Type parameter);

            public abstract void Write(XmlWriter writer, object instance);
            public abstract object Read(XElement element);
        }

        class IntegerConverter : PrimitiveConverterBase<int>
        {
            public static IntegerConverter Default { get; } = new IntegerConverter();
            IntegerConverter() : this(Identities.Default) {}

            public IntegerConverter(IIdentities identities) : base(identities, XmlConvert.ToString, XmlConvert.ToInt32) {}
        }

        abstract class PrimitiveConverterBase<T> : Converter<T>
        {
            readonly private static Type SupportedType = typeof(T);

            protected PrimitiveConverterBase(IIdentities provider, Func<T, string> serialize, Func<string, T> deserialize)
                : this(IsAssignableSpecification<T>.Default, new ElementWriter(provider.Get(SupportedType), new ValueWriter<T>(serialize)), new ValueReader<T>(deserialize)) {}

            protected PrimitiveConverterBase(ISpecification<Type> specification, IWriter writer, IReader reader) : base(specification, writer, reader) {}
        }

        abstract class WriterBase<T> : IWriter
        {
            protected abstract void Write(XmlWriter writer, T instance);

            void IWriter.Write(XmlWriter writer, object instance) => Write(writer, (T) instance);
        }

        abstract class WriterBase : IWriter
        {
            public abstract void Write(XmlWriter writer, object instance);
        }

        interface IConditionalWriter : ISpecification<Type>, IWriter {}

        /*class ConditionalWriter : DecoratedWriter, IConditionalWriter
        {
            private readonly Type _supportedType;

            public ConditionalWriter(Type supportedType, IWriter writer) : base(writer)
            {
                _supportedType = supportedType;
            }

            public bool IsSatisfiedBy(Type parameter) => _supportedType.IsAssignableFrom(parameter);
        }*/

        interface IWriter
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

            protected override void Write(XmlWriter writer, T instance) => writer.WriteString(_serialize(instance));
        }

        class DecoratedWriter : WriterBase
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
            private readonly XName _name;

            public ElementWriter(XName name, IWriter writer) : base(writer)
            {
                _name = name;
            }

            public override void Write(XmlWriter writer, object instance)
            {
                writer.WriteStartElement(_name.LocalName, _name.NamespaceName);
                base.Write(writer, instance);
                writer.WriteEndElement();
            }
        }
    }
}