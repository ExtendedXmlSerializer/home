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
using System.Globalization;
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
            /*if (result == null)
            {
                throw new SerializationException($"Could not find Type information from provided value: {value}");
            }*/

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
        AllNames() : this(Identities.Default, LegacyTypeNames.Default) {}

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

    class LegacyTypeNames : INames
    {
        public static LegacyTypeNames Default { get; } = new LegacyTypeNames();

        LegacyTypeNames() : this(
            new TypeName(
                new AnySpecification<TypeInfo>(IsArraySpecification.Default,
                                               new AllSpecification<TypeInfo>(IsGenericTypeSpecification.Default,
                                                                              IsEnumerableTypeSpecification.Default)),
                LegacyEnumerableNameProvider.Default),
            new TypeName(AlwaysSpecification<TypeInfo>.Default, NameProvider.Default)) {}

        private readonly ITypeName[] _names;

        public LegacyTypeNames(params ITypeName[] names)
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

    public interface IConverter : IReader, IWriter {}

    public interface ITypeConverter : ICandidate<TypeInfo, IConverter> {}

    public interface IWriter
    {
        void Write(XmlWriter writer, object instance);
    }

    public class Converter : ConverterBase
    {
        private readonly IReader _reader;
        private readonly IWriter _writer;

        public Converter(IReader reader, IWriter writer)
        {
            _reader = reader;
            _writer = writer;
        }

        public override object Read(XElement element, Typed? hint = null) => _reader.Read(element, hint);

        public override void Write(XmlWriter writer, object instance)
            => _writer.Write(writer, instance);
    }

    public class TypeConverter : FixedCandidate<TypeInfo, IConverter>, ITypeConverter
    {
        public TypeConverter(ISpecification<TypeInfo> specification, IReader reader, IWriter writer)
            : this(specification, new Converter(reader, writer)) {}

        public TypeConverter(ISpecification<TypeInfo> specification, IConverter converter)
            : base(specification, converter) {}
    }

    public abstract class ConverterBase : IConverter
    {
        public abstract void Write(XmlWriter writer, object instance);
        public abstract object Read(XElement element, Typed? hint = null);
    }

    class LegacyPrimitives : IEnumerable<ITypeConverter>
    {
        public static LegacyPrimitives Default { get; } = new LegacyPrimitives();
        LegacyPrimitives() {}

        public IEnumerator<ITypeConverter> GetEnumerator()
        {
            yield return LegacyBooleanTypeConverter.Default;
            yield return LegacyCharacterTypeConverter.Default;
            yield return ByteTypeConverter.Default;
            yield return UnsignedByteTypeConverter.Default;
            yield return ShortTypeConverter.Default;
            yield return UnsignedShortTypeConverter.Default;
            yield return IntegerTypeConverter.Default;
            yield return UnsignedIntegerTypeConverter.Default;
            yield return LongTypeConverter.Default;
            yield return UnsignedLongTypeConverter.Default;
            yield return FloatTypeConverter.Default;
            yield return DoubleTypeConverter.Default;
            yield return DecimalTypeConverter.Default;
            yield return EnumerationTypeConverter.Default;
            yield return DateTimeTypeConverter.Default;
            yield return DateTimeOffsetTypeConverter.Default;
            yield return StringTypeConverter.Default;
            yield return GuidTypeConverter.Default;
            yield return TimeSpanTypeConverter.Default;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public interface ITypeConverters : IParameterizedSource<IConverter, IEnumerable<ITypeConverter>> {}

    class LegacyAdditionalTypeConverters : ITypeConverters
    {
        private readonly ITypes _types;

        public LegacyAdditionalTypeConverters(ITypes types)
        {
            _types = types;
        }

        public IEnumerable<ITypeConverter> Get(IConverter parameter)
        {
            yield return new LegacyDictionaryTypeConverter(_types, parameter);
            yield return new LegacyArrayTypeConverter(_types, parameter);
            yield return new LegacyEnumerableTypeConverter(_types, parameter);
            yield return new LegacyInstanceTypeConverter(_types, parameter);
        }
    }

    public interface ISelectorFactory : IParameterizedSource<IConverter, ISelector> {}

    public class SelectorFactory : ISelectorFactory
    {
        private readonly IEnumerable<ITypeConverter> _primitives;
        private readonly ITypeConverters _additional;

        public SelectorFactory(IEnumerable<ITypeConverter> primitives, ITypeConverters additional)
        {
            _primitives = primitives;
            _additional = additional;
        }

        public ISelector Get(IConverter parameter) => new Selector(_primitives.Concat(_additional.Get(parameter)));
    }

    class LegacySelectorFactory : AlteredSelectorFactory
    {
        public static LegacySelectorFactory Default { get; } = new LegacySelectorFactory();
        LegacySelectorFactory() : this(Types.Default) {}

        public LegacySelectorFactory(ITypes types)
            : base(
                new SelectorFactory(LegacyPrimitives.Default, new LegacyAdditionalTypeConverters(types)),
                NullableSelectorAlteration.Default) {}
    }

    public class AlteredSelectorFactory : ISelectorFactory
    {
        private readonly ISelectorFactory _factory;
        private readonly IAlteration<ISelector> _alteration;

        public AlteredSelectorFactory(ISelectorFactory factory, IAlteration<ISelector> alteration)
        {
            _factory = factory;
            _alteration = alteration;
        }

        public ISelector Get(IConverter parameter) => _alteration.Get(_factory.Get(parameter));
    }

    class NullableSelectorAlteration : IAlteration<ISelector>
    {
        public static NullableSelectorAlteration Default { get; } = new NullableSelectorAlteration();
        NullableSelectorAlteration() {}

        public ISelector Get(ISelector parameter) => new NullableAwareSelector(parameter);
    }

    public class AssignableSelector : ISelector, ICommand<ISelector>
    {
        private ISelector _selector;

        public IConverter Get(TypeInfo parameter) => _selector?.Get(parameter);
        public void Execute(ISelector parameter) => _selector = parameter;
    }

    public interface IRootConverters<in T> : IParameterizedSource<T, IConverter> {}

    public class RootConverter
    {
        public static IConverter Default { get; } = RootConverters.Default.Get(new object());
        RootConverter() {}
    }

    public class RootConverters : RootConverters<object>
    {
        public static RootConverters Default { get; } = new RootConverters();
        RootConverters() : base(Types.Default, LegacySelectorFactory.Default) {}
    }

    class LegacyRootConverters : RootConverters<ISerializationToolsFactory>
    {
        public static LegacyRootConverters Default { get; } = new LegacyRootConverters();
        LegacyRootConverters() : this(Types.Default) {}

        public LegacyRootConverters(ITypes types) : base(types, new LegacySelectorFactory(types)) {}

        protected override IConverter Create(ISerializationToolsFactory parameter) =>
            new LegacyRootConverter(parameter, base.Create(parameter));
    }

    class CustomElementWriter : ElementWriter
    {
        public CustomElementWriter(IExtendedXmlSerializerConfig configuration)
            : this(AllNames.Default, new TypeEmittingWriter(new CustomElementBodyWriter(configuration))) {}

        public CustomElementWriter(INames names, IWriter writer) : base(names.Get, writer) {}

        sealed class CustomElementBodyWriter : WriterBase
        {
            private readonly IExtendedXmlSerializerConfig _configuration;

            public CustomElementBodyWriter(IExtendedXmlSerializerConfig configuration)
            {
                _configuration = configuration;
            }

            public override void Write(XmlWriter writer, object instance)
                => _configuration.WriteObject(writer, instance);
        }
    }

    class LegacyRootConverter : DecoratedConverter
    {
        private readonly ISerializationToolsFactory _factory;

        public LegacyRootConverter(ISerializationToolsFactory factory, IConverter converter) : base(converter)
        {
            _factory = factory;
        }

        public override void Write(XmlWriter writer, object instance)
        {
            var configuration = _factory.GetConfiguration(instance.GetType());
            if (configuration != null)
            {
                if (configuration.Version > 0)
                {
                    writer.WriteAttributeString(ExtendedXmlSerializer.Version,
                                                configuration.Version.ToString(CultureInfo.InvariantCulture));
                }

                if (configuration.IsCustomSerializer)
                {
                    new CustomElementWriter(configuration).Write(writer, instance);
                    return;
                }
            }

            base.Write(writer, instance);
        }

        public override object Read(XElement element, Typed? hint = null)
        {
            var configuration = _factory.GetConfiguration(hint);
            if (configuration != null)
            {
                // Run migrator if exists
                if (configuration.Version > 0)
                {
                    configuration.Map(hint, element);
                }

                if (configuration.IsCustomSerializer)
                {
                    return configuration.ReadObject(element);
                }
            }

            return base.Read(element, hint);
        }
    }

    public class DecoratedConverter : ConverterBase
    {
        private readonly IConverter _converter;

        public DecoratedConverter(IConverter converter)
        {
            _converter = converter;
        }

        public override void Write(XmlWriter writer, object instance) => _converter.Write(writer, instance);

        public override object Read(XElement element, Typed? hint = null) => _converter.Read(element, hint);
    }

    public class RootConverters<T> : WeakCacheBase<T, IConverter>, IRootConverters<T> where T : class
    {
        private readonly ITypes _types;
        private readonly ISelectorFactory _selector;

        public RootConverters(ITypes types, ISelectorFactory selector)
        {
            _types = types;
            _selector = selector;
        }

        protected override IConverter Create(T parameter)
        {
            var source = new AssignableSelector();
            var result = Create(parameter, source);

            source.Execute(_selector.Get(result));

            return result;
        }

        protected virtual Converter Create(T parameter, ISelector source) =>
            new Converter(new SelectingReader(_types, source), new SelectingWriter(source));
    }

    public interface ISelector : ISelector<TypeInfo, IConverter> {}

    public class Selector : Selector<TypeInfo, IConverter>, ISelector
    {
        public Selector(IEnumerable<ITypeConverter> candidates) : base(candidates.ToArray()) {}
    }

    public class NullableAwareSelector : ISelector
    {
        private readonly ISelector _selector;

        public NullableAwareSelector(ISelector selector)
        {
            _selector = selector;
        }

        public IConverter Get(TypeInfo parameter) => _selector.Get(parameter.AccountForNullable());
    }

    public class TimeSpanTypeConverter : PrimitiveTypeConverterBase<TimeSpan>
    {
        public static TimeSpanTypeConverter Default { get; } = new TimeSpanTypeConverter();
        TimeSpanTypeConverter() : base(XmlConvert.ToString, XmlConvert.ToTimeSpan) {}
    }

    public class GuidTypeConverter : PrimitiveTypeConverterBase<Guid>
    {
        public static GuidTypeConverter Default { get; } = new GuidTypeConverter();
        GuidTypeConverter() : base(XmlConvert.ToString, XmlConvert.ToGuid) {}
    }

    public class DateTimeOffsetTypeConverter : PrimitiveTypeConverterBase<DateTimeOffset>
    {
        public static DateTimeOffsetTypeConverter Default { get; } = new DateTimeOffsetTypeConverter();
        DateTimeOffsetTypeConverter() : base(XmlConvert.ToString, XmlConvert.ToDateTimeOffset) {}
    }

    public class DateTimeTypeConverter : PrimitiveTypeConverterBase<DateTime>
    {
        public static DateTimeTypeConverter Default { get; } = new DateTimeTypeConverter();

        DateTimeTypeConverter()
            : base(
                x => XmlConvert.ToString(x, XmlDateTimeSerializationMode.RoundtripKind),
                x => XmlConvert.ToDateTime(x, XmlDateTimeSerializationMode.RoundtripKind)) {}
    }

    public class DecimalTypeConverter : PrimitiveTypeConverterBase<decimal>
    {
        public static DecimalTypeConverter Default { get; } = new DecimalTypeConverter();
        DecimalTypeConverter() : base(XmlConvert.ToString, XmlConvert.ToDecimal) {}
    }

    public class DoubleTypeConverter : PrimitiveTypeConverterBase<double>
    {
        public static DoubleTypeConverter Default { get; } = new DoubleTypeConverter();
        DoubleTypeConverter() : base(XmlConvert.ToString, XmlConvert.ToDouble) {}
    }

    public class FloatTypeConverter : PrimitiveTypeConverterBase<float>
    {
        public static FloatTypeConverter Default { get; } = new FloatTypeConverter();
        FloatTypeConverter() : base(XmlConvert.ToString, XmlConvert.ToSingle) {}
    }

    public class LongTypeConverter : PrimitiveTypeConverterBase<long>
    {
        public static LongTypeConverter Default { get; } = new LongTypeConverter();
        LongTypeConverter() : base(XmlConvert.ToString, XmlConvert.ToInt64) {}
    }

    public class UnsignedLongTypeConverter : PrimitiveTypeConverterBase<ulong>
    {
        public static UnsignedLongTypeConverter Default { get; } = new UnsignedLongTypeConverter();
        UnsignedLongTypeConverter() : base(XmlConvert.ToString, XmlConvert.ToUInt64) {}
    }

    public class ByteTypeConverter : PrimitiveTypeConverterBase<sbyte>
    {
        public static ByteTypeConverter Default { get; } = new ByteTypeConverter();
        ByteTypeConverter() : base(XmlConvert.ToString, XmlConvert.ToSByte) {}
    }

    public class UnsignedByteTypeConverter : PrimitiveTypeConverterBase<byte>
    {
        public static UnsignedByteTypeConverter Default { get; } = new UnsignedByteTypeConverter();
        UnsignedByteTypeConverter() : base(XmlConvert.ToString, XmlConvert.ToByte) {}
    }

    public class ShortTypeConverter : PrimitiveTypeConverterBase<short>
    {
        public static ShortTypeConverter Default { get; } = new ShortTypeConverter();
        ShortTypeConverter() : base(XmlConvert.ToString, XmlConvert.ToInt16) {}
    }

    public class UnsignedShortTypeConverter : PrimitiveTypeConverterBase<ushort>
    {
        public static UnsignedShortTypeConverter Default { get; } = new UnsignedShortTypeConverter();
        UnsignedShortTypeConverter() : base(XmlConvert.ToString, XmlConvert.ToUInt16) {}
    }

    public class IntegerTypeConverter : PrimitiveTypeConverterBase<int>
    {
        public static IntegerTypeConverter Default { get; } = new IntegerTypeConverter();
        IntegerTypeConverter() : base(XmlConvert.ToString, XmlConvert.ToInt32) {}
    }

    public class UnsignedIntegerTypeConverter : PrimitiveTypeConverterBase<uint>
    {
        public static UnsignedIntegerTypeConverter Default { get; } = new UnsignedIntegerTypeConverter();
        UnsignedIntegerTypeConverter() : base(XmlConvert.ToString, XmlConvert.ToUInt32) {}
    }

    public class BooleanTypeConverter : PrimitiveTypeConverterBase<bool>
    {
        public static BooleanTypeConverter Default { get; } = new BooleanTypeConverter();
        BooleanTypeConverter() : base(XmlConvert.ToString, XmlConvert.ToBoolean) {}
    }

    public class LegacyBooleanTypeConverter : PrimitiveTypeConverterBase<bool>
    {
        public static LegacyBooleanTypeConverter Default { get; } = new LegacyBooleanTypeConverter();
        LegacyBooleanTypeConverter() : base(x => x.ToString(), Convert.ToBoolean) {}
    }

    public class CharacterTypeConverter : PrimitiveTypeConverterBase<char>
    {
        public static CharacterTypeConverter Default { get; } = new CharacterTypeConverter();
        CharacterTypeConverter() : base(XmlConvert.ToString, XmlConvert.ToChar) {}
    }

    public class LegacyCharacterTypeConverter : PrimitiveTypeConverterBase<char>
    {
        public static LegacyCharacterTypeConverter Default { get; } = new LegacyCharacterTypeConverter();
        LegacyCharacterTypeConverter() : base(x => XmlConvert.ToString((ushort) x), s => (char) XmlConvert.ToUInt16(s)) {}
    }

    public class StringTypeConverter : PrimitiveTypeConverterBase<string>
    {
        readonly private static Func<string, string> Self = Self<string>.Default.Get;

        public static StringTypeConverter Default { get; } = new StringTypeConverter();
        StringTypeConverter() : base(Self, Self) {}
    }

    class TypeEmittingWriter : DecoratedWriter
    {
        private readonly ISpecification<TypeInfo> _specification;

        public TypeEmittingWriter(IWriter writer) : this(AlwaysSpecification<TypeInfo>.Default, writer) {}

        public TypeEmittingWriter(ISpecification<TypeInfo> specification, IWriter writer) : base(writer)
        {
            _specification = specification;
        }

        public override void Write(XmlWriter writer, object instance)
        {
            var type = new Typed(instance.GetType());
            var tracker = Tracker.Default.Get(writer);
            if (_specification.IsSatisfiedBy(type) && !tracker.Contains(instance))
            {
                tracker.Add(instance);
                writer.WriteAttributeString(ExtendedXmlSerializer.Type,
                                            LegacyTypeFormatter.Default.Format(type));
            }

            base.Write(writer, instance);
        }

        sealed class Tracker : WeakCache<XmlWriter, HashSet<object>>
        {
            public static Tracker Default { get; } = new Tracker();
            Tracker() : base(_ => new HashSet<object>()) {}
        }
    }

    public class EnumerationTypeConverter : PrimitiveTypeConverterBase<Enum>
    {
        public static EnumerationTypeConverter Default { get; } = new EnumerationTypeConverter();

        EnumerationTypeConverter()
            : base(IsAssignableSpecification<Enum>.Default, new ValueWriter<Enum>(x => x.ToString()), EnumReader.Default
            ) {}
    }


    sealed class ListAdapter : IList
    {
        private readonly object _instance;
        private readonly Action<object, object> _add;

        public ListAdapter(object instance, Action<object, object> add)
        {
            _instance = instance;
            _add = add;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotSupportedException();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotSupportedException();
        }

        int ICollection.Count
        {
            get { throw new NotSupportedException(); }
        }

        object ICollection.SyncRoot
        {
            get { throw new NotSupportedException(); }
        }

        bool ICollection.IsSynchronized
        {
            get { throw new NotSupportedException(); }
        }

        int IList.Add(object value)
        {
            _add(_instance, value);
            return 0;
        }

        bool IList.Contains(object value)
        {
            throw new NotSupportedException();
        }

        void IList.Clear()
        {
            throw new NotSupportedException();
        }

        int IList.IndexOf(object value)
        {
            throw new NotSupportedException();
        }

        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        void IList.Remove(object value)
        {
            throw new NotSupportedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        object IList.this[int index]
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        bool IList.IsReadOnly
        {
            get { throw new NotSupportedException(); }
        }

        bool IList.IsFixedSize
        {
            get { throw new NotSupportedException(); }
        }
    }

    public class LegacyArrayTypeConverter : TypeConverter
    {
        public LegacyArrayTypeConverter(ITypes types, IConverter converter)
            : base(IsArraySpecification.Default, new ArrayReader(types, converter), new EnumerableBodyWriter(converter)) {}
    }

    public class LegacyEnumerableTypeConverter : TypeConverter
    {
        public LegacyEnumerableTypeConverter(ITypes types, IConverter converter)
            : base(
                IsEnumerableTypeSpecification.Default,
                new ListReader(types, converter),
                new EnumerableBodyWriter(converter)
            ) {}
    }

    public class LegacyDictionaryTypeConverter : TypeConverter
    {
        public LegacyDictionaryTypeConverter(ITypes types, IConverter converter)
            : base(
                IsAssignableSpecification<IDictionary>.Default, new DictionaryReader(types, converter),
                new DictionaryBodyWriter(converter)) {}
    }

    public class IsArraySpecification : ISpecification<TypeInfo>
    {
        public static IsArraySpecification Default { get; } = new IsArraySpecification();
        IsArraySpecification() {}

        public bool IsSatisfiedBy(TypeInfo parameter) => parameter.IsArray;
    }

    public class IsGenericTypeSpecification : ISpecification<TypeInfo>
    {
        public static IsGenericTypeSpecification Default { get; } = new IsGenericTypeSpecification();
        IsGenericTypeSpecification() {}

        public bool IsSatisfiedBy(TypeInfo parameter) => parameter.IsGenericType;
    }

    public class IsEnumerableTypeSpecification : IsAssignableSpecification<IEnumerable>
    {
        public static new IsEnumerableTypeSpecification Default { get; } = new IsEnumerableTypeSpecification();
        IsEnumerableTypeSpecification() {}
    }

    public static class Extensions
    {
        public static TypeInfo AccountForNullable(this TypeInfo @this)
            => Nullable.GetUnderlyingType(@this.AsType())?.GetTypeInfo() ?? @this;

        public static Type AccountForNullable(this Type @this) => Nullable.GetUnderlyingType(@this) ?? @this;

        public static T Activate<T>(this IActivators @this, Typed type) => (T) @this.Get(type).Invoke();
    }

    class InstanceBodyReader : ReaderBase
    {
        private readonly IInstanceMembers _members;
        private readonly ITypes _types;
        private readonly IActivators _activators;

        public InstanceBodyReader(IInstanceMembers members, ITypes types, IActivators activators)
        {
            _members = members;
            _types = types;
            _activators = activators;
        }

        public override object Read(XElement element, Typed? hint = null)
        {
            var type = hint ?? _types.Get(element);
            var result = type.HasValue ? Create(element, type.Value) : null;
            return result;
        }

        protected virtual object Create(XElement element, Typed type)
        {
            var result = _activators.Activate<object>(type);
            OnRead(element, result, type);
            return result;
        }

        protected virtual void OnRead(XElement element, object result, Typed type)
        {
            var members = _members.Get(type);
            foreach (var child in element.Elements())
            {
                var member = members.Get(child.Name);
                if (member != null)
                {
                    Apply(result, member, member.Read(child, _types.Get(child)));
                }
            }
        }

        protected virtual void Apply(object instance, IMember member, object value)
        {
            if (value != null)
            {
                var assignable = member as IAssignableMember;
                assignable?.Set(instance, value);
            }
        }
    }

    class InstanceBodyWriter : WriterBase
    {
        private readonly IInstanceMembers _members;

        public InstanceBodyWriter(IInstanceMembers members)
        {
            _members = members;
        }

        public override void Write(XmlWriter writer, object instance)
        {
            foreach (var member in _members.Get(instance.GetType().GetTypeInfo()))
            {
                member.Write(writer, instance);
            }
        }
    }

    public class LegacyInstanceTypeConverter : TypeConverter
    {
        public LegacyInstanceTypeConverter(ITypes types, IConverter converter)
            : this(IsActivatedTypeSpecification.Default, types, converter) {}

        protected LegacyInstanceTypeConverter(ISpecification<TypeInfo> specification, ITypes types,
                                              IConverter converter)
            : this(
                specification, new InstanceMembers(converter, new EnumeratingReader(types, converter)), types,
                Activators.Default) {}

        public LegacyInstanceTypeConverter(ISpecification<TypeInfo> specification, IInstanceMembers members,
                                           ITypes types,
                                           IActivators activators)
            : base(
                specification, new InstanceBodyReader(members, types, activators),
                new TypeEmittingWriter(new InstanceBodyWriter(members))) {}
    }


    public class IsActivatedTypeSpecification : ISpecification<TypeInfo>
    {
        public static IsActivatedTypeSpecification Default { get; } = new IsActivatedTypeSpecification();
        IsActivatedTypeSpecification() {}

        public bool IsSatisfiedBy(TypeInfo parameter)
            => parameter.IsValueType ||
               !parameter.IsAbstract && parameter.IsClass && parameter.GetConstructor(Type.EmptyTypes) != null;
    }

    public class EmitTypeSpecification : ISpecification<TypeInfo>
    {
        public static EmitTypeSpecification Default { get; } = new EmitTypeSpecification();
        EmitTypeSpecification() {}

        public bool IsSatisfiedBy(TypeInfo parameter)
            => !parameter.IsPrimitive &&
               !parameter.IsArray &&
               !typeof(IEnumerable).IsAssignableFrom(parameter.AsType()) &
               IsActivatedTypeSpecification.Default.IsSatisfiedBy(parameter);
    }

    public abstract class PrimitiveTypeConverterBase<T> : TypeConverter
    {
        protected PrimitiveTypeConverterBase(Func<T, string> serialize,
                                             Func<string, T> deserialize)
            : this(
                new ValueWriter<T>(serialize),
                new ValueReader<T>(deserialize)
            ) {}

        protected PrimitiveTypeConverterBase(IWriter writer, IReader reader)
            : this(TypeEqualitySpecification<T>.Default, writer, reader) {}

        protected PrimitiveTypeConverterBase(ISpecification<TypeInfo> specification, IWriter writer, IReader reader)
            : base(specification, new ValueValidatingReader(reader), new InstanceValidatingWriter(writer)) {}
    }
}