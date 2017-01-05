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
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.Model.Write;
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
                       {typeof(int), XName.Get("int", namespaceName)},
                       {typeof(string), XName.Get("string", namespaceName)}
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

    public interface INames : IParameterizedSource<TypeInfo, XName> {}

    public class Names : INames
    {
        public static Names Default { get; } = new Names();
        Names() : this(Identities.Default, TypeNames.Default) {}

        private readonly IIdentities _identities;
        private readonly INames _types;

        public Names(IIdentities identities, INames types)
        {
            _identities = identities;
            _types = types;
        }

        public XName Get(TypeInfo parameter) => _identities.Get(parameter.AsType()) ?? _types.Get(parameter);
    }

    public interface ITypeName : ISpecification<TypeInfo>, IParameterizedSource<TypeInfo, XName> {}

    class TypeName : ITypeName
    {
        private readonly ISpecification<TypeInfo> _specification;
        private readonly IParameterizedSource<TypeInfo, XName> _source;

        public TypeName(ISpecification<TypeInfo> specification, IParameterizedSource<TypeInfo, XName> source)
        {
            _specification = specification;
            _source = source;
        }

        public bool IsSatisfiedBy(TypeInfo parameter) => _specification.IsSatisfiedBy(parameter);

        public XName Get(TypeInfo parameter) => _source.Get(parameter);
    }

    public class TypeNames : WeakCacheBase<TypeInfo, XName>, INames
    {
        public static TypeNames Default { get; } = new TypeNames();
        TypeNames() : this(new TypeName(IsActivatedTypeSpecification.Default, NameProvider.Default)) {}

        private readonly ITypeName[] _names;

        public TypeNames(params ITypeName[] names)
        {
            _names = names;
        }

        protected override XName Create(TypeInfo parameter)
        {
            foreach (var name in _names)
            {
                if (name.IsSatisfiedBy(parameter))
                {
                    return name.Get(parameter);
                }
            }
            return null;
        }
    }

    public interface IConverter : ISpecification<TypeInfo>, IWriter, IReader {}

    public interface IWriter
    {
        void Write(XmlWriter writer, object instance);
    }

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

        public override object Read(XElement element) => _reader.Read(element);

        public override void Write(XmlWriter writer, object instance)
            => _writer.Write(writer, instance);
    }

    public abstract class ConverterBase : IConverter
    {
        public abstract bool IsSatisfiedBy(TypeInfo parameter);

        public abstract void Write(XmlWriter writer, object instance);
        public abstract object Read(XElement element);
    }

    class KnownConverters : IEnumerable<IConverter>
    {
        public static KnownConverters Default { get; } = new KnownConverters();
        KnownConverters() {}

        public IEnumerator<IConverter> GetEnumerator()
        {
            yield return IntegerConverter.Default;
            yield return StringConverter.Default;
            yield return ActivatedTypeConverter.Default;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class Root : ElementWriter
    {
        public static Root Default { get; } = new Root();
        Root() : this(Names.Default) {}

        public Root(INames names) : this(names, SelectedWriter.Default) {}

        public Root(INames names, IWriter body) : base(names.Get, body) {}
    }

    public interface ISelector : IParameterizedSource<TypeInfo, IConverter> {}

    public class Selector : WeakCacheBase<TypeInfo, IConverter>, ISelector //, IReader, IWriter
    {
        public static Selector Default { get; } = new Selector();
        Selector() : this(KnownConverters.Default.ToArray()) {}

        private readonly ImmutableArray<IConverter> _converters;

        public Selector(params IConverter[] converters)
            : this(converters.ToImmutableArray()) {}

        public Selector(ImmutableArray<IConverter> converters)
        {
            _converters = converters;
        }

        /*public void Write(XmlWriter writer, object instance)
        {
            var type = new Typed(instance.GetType());
            Get(type).Write(new WriterContext(this, writer), instance, type);
        }

        public object Read(XElement element)
        {
            var info = _provider.Get(element).GetTypeInfo();
            var converter = Get(info);
            var result = converter.Read(element);
            return result;
        }*/

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

        public static StringConverter Default { get; } = new StringConverter();
        StringConverter() : base(Self, Self) {}
    }

    public class IntegerConverter : PrimitiveConverterBase<int>
    {
        public static IntegerConverter Default { get; } = new IntegerConverter();
        IntegerConverter() : base(XmlConvert.ToString, XmlConvert.ToInt32) {}
    }

    public class ActivatedTypeConverter : Converter
    {
        public static ActivatedTypeConverter Default { get; } = new ActivatedTypeConverter();

        ActivatedTypeConverter()
            : base(IsActivatedTypeSpecification.Default, ActivatedTypeWriter.Default, new Reader()) {}
    }


    public class IsActivatedTypeSpecification : ISpecification<TypeInfo>
    {
        public static IsActivatedTypeSpecification Default { get; } = new IsActivatedTypeSpecification();
        IsActivatedTypeSpecification() {}

        public bool IsSatisfiedBy(TypeInfo parameter) => !parameter.IsAbstract &&
                                                         (parameter.IsValueType ||
                                                          parameter.IsClass &&
                                                          parameter.GetConstructor(Type.EmptyTypes) != null);
    }

    /*class TypeOfSpecification<T> : ISpecification<TypeInfo> where T : IType
    {
        private readonly ITypes _types;

        public TypeOfSpecification(ITypes types)
        {
            _types = types;
        }

        public bool IsSatisfiedBy(TypeInfo parameter) => _types.Get(parameter) is T;
    }*/

    public abstract class PrimitiveConverterBase<T> : Converter
    {
        protected PrimitiveConverterBase(Func<T, string> serialize,
                                         Func<string, T> deserialize)
            : this(
                TypeEqualitySpecification<T>.Default,
                new ValueWriter<T>(serialize),
                new ValueReader<T>(deserialize)
            ) {}

        protected PrimitiveConverterBase(ISpecification<TypeInfo> specification, IWriter writer, IReader reader)
            : base(specification, writer, reader) {}
    }
}