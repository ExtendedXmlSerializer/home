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
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.Processing;

namespace ExtendedXmlSerialization.Model
{
    public interface ITypes : IParameterizedSource<XElement, Type> {}

    /*public class HintedRootTypes : ITypes
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
    }*/

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

        TypeNames() : this(
            new TypeName(IsEnumerableTypeSpecification.Default, EnumerableNameProvider.Default),
            new TypeName(IsActivatedTypeSpecification.Default, NameProvider.Default)) {}

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

        public override object Read(XElement element, Typed? hint = null) => _reader.Read(element, hint);

        public override void Write(XmlWriter writer, object instance)
            => _writer.Write(writer, instance);
    }

    public abstract class ConverterBase : IConverter
    {
        public abstract bool IsSatisfiedBy(TypeInfo parameter);

        public abstract void Write(XmlWriter writer, object instance);
        public abstract object Read(XElement element, Typed? hint = null);
    }

    public interface ISelectors : IParameterizedSource<ITypes, ISelector> {}

    class LegacyPrimitiveConverters : IEnumerable<IConverter>
    {
        public static LegacyPrimitiveConverters Default { get; } = new LegacyPrimitiveConverters();
        LegacyPrimitiveConverters() {}

        public IEnumerator<IConverter> GetEnumerator()
        {
            yield return LegacyBooleanConverter.Default;
            yield return CharacterConverter.Default;
            yield return ByteConverter.Default;
            yield return UnsignedByteConverter.Default;
            yield return ShortConverter.Default;
            yield return UnsignedShortConverter.Default;
            yield return IntegerConverter.Default;
            yield return UnsignedIntegerConverter.Default;
            yield return LongConverter.Default;
            yield return UnsignedLongConverter.Default;
            yield return FloatConverter.Default;
            yield return DoubleConverter.Default;
            yield return DecimalConverter.Default;
            yield return EnumerationConverter.Default;
            yield return DateTimeConverter.Default;
            yield return DateTimeOffsetConverter.Default;
            yield return StringConverter.Default;
            yield return GuidConverter.Default;
            yield return TimeSpanConverter.Default;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    class Selectors : WeakCacheBase<ITypes, ISelector>, ISelectors
    {
        public static Selectors Default { get; } = new Selectors();
        Selectors() : this(LegacyPrimitiveConverters.Default) {}

        private readonly IEnumerable<IConverter> _primitives;

        public Selectors(IEnumerable<IConverter> primitives)
        {
            _primitives = primitives;
        }

        protected override ISelector Create(ITypes parameter)
        {
            var source = new Source(this, parameter);
            var singleton = new SingletonSource<ISelector>(source.Get);
            var converters = _primitives.Concat(Yield(parameter, singleton.Get)).ToImmutableArray();
            var result = new Selector(converters);
            return result;
        }

        protected virtual IEnumerable<IConverter> Yield(ITypes parameter, Func<ISelector> selector)
        {
            yield return new LegacyEnumerableTypeConverter(parameter, selector);
            yield return new ActivatedTypeConverter(parameter, selector);
        }

        sealed class Source : ISource<ISelector>
        {
            private readonly ISelectors _selectors;
            private readonly ITypes _parameter;

            public Source(ISelectors selectors, ITypes parameter)
            {
                _selectors = selectors;
                _parameter = parameter;
            }

            public ISelector Get() => _selectors.Get(_parameter);
        }
    }

    public interface ISelector : IParameterizedSource<TypeInfo, IConverter> {}

    public class Selector : WeakCacheBase<TypeInfo, IConverter>, ISelector
    {
        private readonly ImmutableArray<IConverter> _converters;

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
            return null;
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

    public class LegacyBooleanConverter : PrimitiveConverterBase<bool>
    {
        public static LegacyBooleanConverter Default { get; } = new LegacyBooleanConverter();
        LegacyBooleanConverter() : base(x => x.ToString(), Convert.ToBoolean) {}
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

    public class TypeEmittingWriter : DecoratedWriter
    {
        public TypeEmittingWriter(IWriter writer) : base(writer) {}

        public override void Write(XmlWriter writer, object instance)
        {
            writer.WriteAttributeString(ExtendedXmlSerializer.Type,
                                        DefaultTypeFormatter.Default.Format(instance.GetType()));
            base.Write(writer, instance);
        }
    }

    public class EnumerationConverter : PrimitiveConverterBase<Enum>
    {
        public static EnumerationConverter Default { get; } = new EnumerationConverter();
        EnumerationConverter() : base(IsAssignableSpecification<Enum>.Default, new ValueWriter<Enum>(x => x.ToString()), EnumReader.Default) {}
    }

    sealed class EnumReader : ReaderBase
    {
        public static EnumReader Default { get; } = new EnumReader();
        EnumReader() {}

        public override object Read(XElement element, Typed? hint = null)
        {
            if (hint.HasValue)
            {
                return Enum.Parse(hint, element.Value);
            }
            throw new InvalidOperationException(
                $"An attempt was made to read element as an enumeration, but no type was specified.");
        }
    }

    public class LegacyEnumerableTypeConverter : ActivatedTypeConverter<IEnumerable>
    {
        private readonly INames _names;
        private readonly Func<ISelector> _selector;
        private readonly IElementTypeLocator _locator;
        private readonly Lazy<IWriter> _writer;
        private readonly IAddDelegates _add;

        public LegacyEnumerableTypeConverter(ITypes types, Func<ISelector> selector)
            : this(types, AllNames.Default, selector, ElementTypeLocator.Default, AddDelegates.Default) {}

        public LegacyEnumerableTypeConverter(ITypes types, INames names, Func<ISelector> selector,
                                             IElementTypeLocator locator,
                                             IAddDelegates add)
            : base(IsEnumerableTypeSpecification.Default, types, selector)
        {
            _names = names;
            _selector = selector;
            _locator = locator;
            _add = add;
            _writer = new Lazy<IWriter>(Create);
        }

        IWriter Create() => new ElementWriter(_names.Get, new TypeEmittingWriter(new SelectingWriter(_selector())));

        protected override void Write(XmlWriter writer, IEnumerable instance, Typed type)
        {
            var w = _writer.Value;
            foreach (var item in instance)
            {
                w.Write(writer, item);
            }
        }

        protected override void OnRead(XElement element, IEnumerable result, Typed type)
        {
            var add = _add.Get(type);
            var selector = _selector();
            var elementType = _locator.Locate(type);
            var elementReader = selector.Get(elementType.GetTypeInfo());
            foreach (var child in element.Elements())
            {
                var itemType = Get(child);
                var reader = elementType == itemType.Type ? elementReader : selector.Get(itemType);
                var item = reader.Read(child, itemType);
                if (item != null)
                {
                    add(result, item);
                }
            }
        }
    }

    public class IsEnumerableTypeSpecification : ISpecification<TypeInfo>
    {
        public static IsEnumerableTypeSpecification Default { get; } = new IsEnumerableTypeSpecification();
        IsEnumerableTypeSpecification() : this(ElementTypeLocator.Default) {}

        private readonly IElementTypeLocator _locator;

        public IsEnumerableTypeSpecification(IElementTypeLocator locator)
        {
            _locator = locator;
        }

        public bool IsSatisfiedBy(TypeInfo parameter) => _locator.Locate(parameter.AsType()) != null;
    }

    public class ActivatedTypeConverter : ActivatedTypeConverter<object>
    {
        public ActivatedTypeConverter(ITypes types, Func<ISelector> selector) : base(types, selector) {}
    }

    public class ActivatedTypeConverter<T> : ConverterBase
    {
        public ActivatedTypeConverter(ITypes types, Func<ISelector> selector)
            : this(IsActivatedTypeSpecification.Default, types, selector) {}

        private readonly ISpecification<TypeInfo> _specification;
        private readonly IInstanceMembers _members;
        private readonly ITypes _types;
        private readonly IActivators _activators;

        protected ActivatedTypeConverter(ISpecification<TypeInfo> specification, ITypes types, Func<ISelector> selector)
            : this(specification, new InstanceMembers(selector), types, Activators.Default) {}

        public ActivatedTypeConverter(ISpecification<TypeInfo> specification, IInstanceMembers members, ITypes types,
                                      IActivators activators)
        {
            _specification = specification;
            _members = members;
            _types = types;
            _activators = activators;
        }

        public override bool IsSatisfiedBy(TypeInfo parameter) => _specification.IsSatisfiedBy(parameter);

        public sealed override void Write(XmlWriter writer, object instance)
            => Write(writer, (T) instance, instance.GetType());

        protected virtual void Write(XmlWriter writer, T instance, Typed type)
        {
            foreach (var member in _members.Get(type))
            {
                member.Write(writer, instance);
            }
        }

        public sealed override object Read(XElement element, Typed? hint = null)
        {
            var type = hint ?? Get(element);
            var result = _activators.Get(type).Invoke().AsValid<T>();
            OnRead(element, result, type);
            return result;
        }

        protected Typed Get(XElement element) => _types.Get(element);

        protected virtual void OnRead(XElement element, T result, Typed type)
        {
            var members = _members.Get(type);
            foreach (var child in element.Elements())
            {
                var member = members.Get(child.Name);
                if (member != null)
                {
                    Apply(result, member, member.Read(child));
                }
            }
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
                new ValueWriter<T>(serialize),
                new ValueReader<T>(deserialize)
            ) {}

        protected PrimitiveConverterBase(IWriter writer, IReader reader)
            : this(TypeEqualitySpecification<T>.Default, writer, reader) {}

        protected PrimitiveConverterBase(ISpecification<TypeInfo> specification, IWriter writer, IReader reader)
            : base(specification, writer, reader) {}
    }
}