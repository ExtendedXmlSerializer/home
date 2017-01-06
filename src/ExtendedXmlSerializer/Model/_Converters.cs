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
    public interface ITypes : IParameterizedSource<XElement, Type> {}

    public class HintedRootTypes : ITypes
    {
        private readonly Type _hint;
        private readonly ITypes _provider;

        public HintedRootTypes(Type hint) : this(hint, Types.Default) {}

        public HintedRootTypes(Type hint, ITypes provider)
        {
            _hint = hint;
            _provider = provider;
        }

        public Type Get(XElement parameter)
            => parameter.Document.Root == parameter ? _hint : _provider.Get(parameter);
    }

    public class Types : WeakCacheBase<XElement, Type>, ITypes
    {
        public static Types Default { get; } = new Types();
        Types() : this(TypeParser.Default, Identities.Default) {}

        private readonly ITypeParser _parser;
        private readonly IIdentities _identities;

        public Types(ITypeParser parser, IIdentities identities)
        {
            _parser = parser;
            _identities = identities;
        }

        protected override Type Create(XElement parameter)
            => _identities.Get(parameter.Name) ?? FromAttribute(parameter);

        private Type FromAttribute(XElement parameter)
        {
            var value = parameter.Attribute(ExtendedXmlSerializer.Type)?.Value;
            var result = value != null ? _parser.Get(value) : null;
            if (result == null)
            {
                throw new SerializationException($"Could not find Type information from provided value: {value}");
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
                       {typeof(bool), XName.Get("boolean", namespaceName)},
                       {typeof(char), XName.Get("char", namespaceName)},
                       {typeof(sbyte), XName.Get("byte", namespaceName)},
                       {typeof(byte), XName.Get("unsignedByte", namespaceName)},
                       {typeof(short), XName.Get("short", namespaceName)},
                       {typeof(ushort), XName.Get("unsignedShort", namespaceName)},
                       {typeof(int), XName.Get("int", namespaceName)},
                       {typeof(uint), XName.Get("unsignedInt", namespaceName)},
                       {typeof(long), XName.Get("long", namespaceName)},
                       {typeof(ulong), XName.Get("unsignedLong", namespaceName)},
                       {typeof(float), XName.Get("float", namespaceName)},
                       {typeof(double), XName.Get("double", namespaceName)},
                       {typeof(decimal), XName.Get("decimal", namespaceName)},
                       {typeof(DateTime), XName.Get("dateTime", namespaceName)},
                       {typeof(DateTimeOffset), XName.Get("dateTimeOffset", namespaceName)},
                       {typeof(string), XName.Get("string", namespaceName)},
                       {typeof(Guid), XName.Get("guid", namespaceName)},
                       {typeof(TimeSpan), XName.Get("TimeSpan", namespaceName)}
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

    public class AllNames : WeakCacheBase<TypeInfo, XName>, INames
    {
        public static AllNames Default { get; } = new AllNames();
        AllNames() : this(Identities.Default, TypeNames.Default) {}

        private readonly IIdentities _identities;
        private readonly INames _types;

        public AllNames(IIdentities identities, INames types)
        {
            _identities = identities;
            _types = types;
        }

        protected override XName Create(TypeInfo parameter)
            => _identities.Get(parameter.AsType()) ?? _types.Get(parameter);
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

    public class TypeNames : INames
    {
        public static TypeNames Default { get; } = new TypeNames();
        TypeNames() : this(new TypeName(IsActivatedTypeSpecification.Default, NameProvider.Default)) {}

        private readonly ITypeName[] _names;

        public TypeNames(params ITypeName[] names)
        {
            _names = names;
        }

        public XName Get(TypeInfo parameter)
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

    public interface ISelector : IParameterizedSource<TypeInfo, IConverter> {}

    public class Selector : WeakCacheBase<TypeInfo, IConverter>, ISelector
    {
        public static Selector Default { get; } = new Selector();
        Selector() : this(Types.Default) {}

        public Selector(ITypes types)
            : this(
                BooleanConverter.Default,
                CharacterConverter.Default,
                ByteConverter.Default,
                UnsignedByteConverter.Default,
                ShortConverter.Default,
                UnsignedShortConverter.Default,
                IntegerConverter.Default,
                UnsignedIntegerConverter.Default,
                LongConverter.Default,
                UnsignedLongConverter.Default,
                FloatConverter.Default,
                DoubleConverter.Default,
                DecimalConverter.Default,
                DateTimeConverter.Default,
                DateTimeOffsetConverter.Default,
                StringConverter.Default,
                GuidConverter.Default,
                TimeSpanConverter.Default,
                new ActivatedTypeConverter(types)
            ) {}

        private readonly ImmutableArray<IConverter> _converters;

        public Selector(params IConverter[] converters)
            : this(converters.ToImmutableArray()) {}

        public Selector(ImmutableArray<IConverter> converters)
        {
            _converters = converters;
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

    public class TimeSpanConverter : PrimitiveConverterBase<TimeSpan>
    {
        public static TimeSpanConverter Default { get; } = new TimeSpanConverter();
        TimeSpanConverter() : base(XmlConvert.ToString, XmlConvert.ToTimeSpan) {}
    }

    public class GuidConverter : PrimitiveConverterBase<Guid>
    {
        public static GuidConverter Default { get; } = new GuidConverter();
        GuidConverter() : base(XmlConvert.ToString, XmlConvert.ToGuid) {}
    }

    public class DateTimeOffsetConverter : PrimitiveConverterBase<DateTimeOffset>
    {
        public static DateTimeOffsetConverter Default { get; } = new DateTimeOffsetConverter();
        DateTimeOffsetConverter() : base(XmlConvert.ToString, XmlConvert.ToDateTimeOffset) {}
    }

    public class DateTimeConverter : PrimitiveConverterBase<DateTime>
    {
        public static DateTimeConverter Default { get; } = new DateTimeConverter();

        DateTimeConverter()
            : base(
                x => XmlConvert.ToString(x, XmlDateTimeSerializationMode.RoundtripKind),
                x => XmlConvert.ToDateTime(x, XmlDateTimeSerializationMode.RoundtripKind)) {}
    }

    public class DecimalConverter : PrimitiveConverterBase<decimal>
    {
        public static DecimalConverter Default { get; } = new DecimalConverter();
        DecimalConverter() : base(XmlConvert.ToString, XmlConvert.ToDecimal) {}
    }

    public class DoubleConverter : PrimitiveConverterBase<double>
    {
        public static DoubleConverter Default { get; } = new DoubleConverter();
        DoubleConverter() : base(XmlConvert.ToString, XmlConvert.ToDouble) {}
    }

    public class FloatConverter : PrimitiveConverterBase<float>
    {
        public static FloatConverter Default { get; } = new FloatConverter();
        FloatConverter() : base(XmlConvert.ToString, XmlConvert.ToSingle) {}
    }

    public class LongConverter : PrimitiveConverterBase<long>
    {
        public static LongConverter Default { get; } = new LongConverter();
        LongConverter() : base(XmlConvert.ToString, XmlConvert.ToInt64) {}
    }

    public class UnsignedLongConverter : PrimitiveConverterBase<ulong>
    {
        public static UnsignedLongConverter Default { get; } = new UnsignedLongConverter();
        UnsignedLongConverter() : base(XmlConvert.ToString, XmlConvert.ToUInt64) {}
    }

    public class ByteConverter : PrimitiveConverterBase<sbyte>
    {
        public static ByteConverter Default { get; } = new ByteConverter();
        ByteConverter() : base(XmlConvert.ToString, XmlConvert.ToSByte) {}
    }

    public class UnsignedByteConverter : PrimitiveConverterBase<byte>
    {
        public static UnsignedByteConverter Default { get; } = new UnsignedByteConverter();
        UnsignedByteConverter() : base(XmlConvert.ToString, XmlConvert.ToByte) {}
    }

    public class ShortConverter : PrimitiveConverterBase<short>
    {
        public static ShortConverter Default { get; } = new ShortConverter();
        ShortConverter() : base(XmlConvert.ToString, XmlConvert.ToInt16) {}
    }

    public class UnsignedShortConverter : PrimitiveConverterBase<ushort>
    {
        public static UnsignedShortConverter Default { get; } = new UnsignedShortConverter();
        UnsignedShortConverter() : base(XmlConvert.ToString, XmlConvert.ToUInt16) {}
    }

    public class IntegerConverter : PrimitiveConverterBase<int>
    {
        public static IntegerConverter Default { get; } = new IntegerConverter();
        IntegerConverter() : base(XmlConvert.ToString, XmlConvert.ToInt32) {}
    }

    public class UnsignedIntegerConverter : PrimitiveConverterBase<uint>
    {
        public static UnsignedIntegerConverter Default { get; } = new UnsignedIntegerConverter();
        UnsignedIntegerConverter() : base(XmlConvert.ToString, XmlConvert.ToUInt32) {}
    }

    public class BooleanConverter : PrimitiveConverterBase<bool>
    {
        public static BooleanConverter Default { get; } = new BooleanConverter();
        BooleanConverter() : base(XmlConvert.ToString, XmlConvert.ToBoolean) {}
    }

    public class CharacterConverter : PrimitiveConverterBase<char>
    {
        public static CharacterConverter Default { get; } = new CharacterConverter();
        CharacterConverter() : base(XmlConvert.ToString, XmlConvert.ToChar) {}
    }

    public class StringConverter : PrimitiveConverterBase<string>
    {
        readonly private static Func<string, string> Self = Self<string>.Default.Get;

        public static StringConverter Default { get; } = new StringConverter();
        StringConverter() : base(Self, Self) {}
    }

    public class ActivatedTypeConverter : ConverterBase
    {
        public static ActivatedTypeConverter Default { get; } = new ActivatedTypeConverter();
        ActivatedTypeConverter() : this(Types.Default) {}

        public ActivatedTypeConverter(ITypes types)
            : this(IsActivatedTypeSpecification.Default, InstanceMembers.Default, types, Activators.Default) {}

        private readonly ISpecification<TypeInfo> _specification;
        private readonly IInstanceMembers _members;
        private readonly ITypes _provider;
        private readonly IActivators _activators;

        public ActivatedTypeConverter(ISpecification<TypeInfo> specification, IInstanceMembers members, ITypes provider,
                                      IActivators activators)
        {
            _specification = specification;
            _members = members;
            _provider = provider;
            _activators = activators;
        }

        public override bool IsSatisfiedBy(TypeInfo parameter) => _specification.IsSatisfiedBy(parameter);

        public override void Write(XmlWriter writer, object instance)
        {
            foreach (var member in _members.Get(instance.GetType().GetTypeInfo()))
            {
                member.Write(writer, instance);
            }
        }

        public override object Read(XElement element)
        {
            var type = _provider.Get(element);
            var result = _activators.Get(type).Invoke();
            var members = _members.Get(type.GetTypeInfo());
            foreach (var child in element.Elements())
            {
                var member = members.Get(child.Name);
                if (member != null)
                {
                    Apply(result, member, member.Read(child));
                }
            }
            return result;
        }

        protected virtual void Apply(object instance, IMember member, object value)
        {
            var assignable = member as IAssignableMember;
            assignable?.Set(instance, value);
        }
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