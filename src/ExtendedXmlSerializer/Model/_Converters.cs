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
using System.Xml.Linq;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.Processing;

namespace ExtendedXmlSerialization.Model
{
    public interface ITypeProvider : IParameterizedSource<XElement, Type> {}

    public class HintedRootTypeProvider : ITypeProvider
    {
        private readonly Type _hint;
        private readonly ITypeProvider _provider;

        public HintedRootTypeProvider(Type hint, ITypeProvider provider)
        {
            _hint = hint;
            _provider = provider;
        }

        public Type Get(XElement parameter)
            => parameter.Document.Root == parameter ? _hint : _provider.Get(parameter);
    }

    public class TypeProvider : ITypeProvider
    {
        public static TypeProvider Default { get; } = new TypeProvider();
        TypeProvider() : this(Processing.Types.Default, Identities.Default) {}

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

    public interface IIdentities : ISpecification<Type>, ISpecification<XName>,
                                   IParameterizedSource<Type, XName>, IParameterizedSource<XName, Type> {}

    public class Identities : IIdentities
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

    public interface IConverter : ISpecification<TypeInfo>, IWriter, IReader {}

    public class Converter : ConverterBase
    {
        private readonly ISpecification<TypeInfo> _specification;
        private readonly IWriter _writer;
        private readonly IReader _reader;

        public Converter(ISpecification<TypeInfo> specification, IWriter writer, IReader reader)
        {
            _specification = specification;
            _writer = writer;
            _reader = reader;
        }

        public override bool IsSatisfiedBy(TypeInfo parameter) => _specification.IsSatisfiedBy(parameter);
        public override void Write(XmlWriter writer, object instance) => _writer.Write(writer, instance);
        public override object Read(XElement element) => _reader.Read(element);
    }

    public abstract class ConverterBase : IConverter
    {
        public abstract bool IsSatisfiedBy(TypeInfo parameter);

        public abstract void Write(XmlWriter writer, object instance);
        public abstract object Read(XElement element);
    }

    class Converters : WeakCacheBase<TypeInfo, IConverter>, IReader, IWriter
    {
        private readonly ITypeProvider _provider;
        private readonly ImmutableArray<IConverter> _converters;

        public Converters(params IConverter[] converters)
            : this(TypeProvider.Default, converters.ToImmutableArray()) {}

        public Converters(ITypeProvider provider, ImmutableArray<IConverter> converters)
        {
            _provider = provider;
            _converters = converters;
        }

        public void Write(XmlWriter writer, object instance)
            => Get(instance.GetType().GetTypeInfo()).Write(writer, instance);

        public object Read(XElement element)
        {
            var info = _provider.Get(element).GetTypeInfo();
            var converter = Get(info);
            var result = converter.Read(element);
            return result;
        }

        protected override IConverter Create(TypeInfo parameter)
        {
            foreach (var converter in _converters)
            {
                if (converter.IsSatisfiedBy(parameter))
                {
                    return converter;
                }
            }
            throw new InvalidOperationException($"Could not find a converter for '{parameter}'");
        }
    }

    public class StringConverter : PrimitiveConverterBase<string>
    {
        readonly private static Func<string, string> Self = Self<string>.Default.Get;
        public StringConverter(IIdentities provider) : base(provider, Self, Self) {}
    }

    public class IntegerConverter : PrimitiveConverterBase<int>
    {
        public IntegerConverter(IIdentities identities) : base(identities, XmlConvert.ToString, XmlConvert.ToInt32) {}
    }

    public abstract class PrimitiveConverterBase<T> : Converter
    {
        readonly private static Type SupportedType = typeof(T);

        protected PrimitiveConverterBase(IIdentities provider, Func<T, string> serialize,
                                         Func<string, T> deserialize)
            : this(
                TypeEqualitySpecification<T>.Default,
                new ElementWriter(provider.Get(SupportedType).Accept, new ValueWriter<T>(serialize)),
                new ElementValueReader<T>(deserialize)
            ) {}

        protected PrimitiveConverterBase(ISpecification<TypeInfo> specification, IWriter writer, IReader reader)
            : base(specification, writer, reader) {}
    }
}