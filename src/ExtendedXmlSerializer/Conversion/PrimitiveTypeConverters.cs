using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerialization.Conversion.Legacy;
using ExtendedXmlSerialization.Conversion.Members;
using ExtendedXmlSerialization.Conversion.Primitives;
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.Conversion.TypeModel;
using ExtendedXmlSerialization.Conversion.Write;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion
{
    public class PrimitiveTypeConverters : IEnumerable<ITypeConverter>
    {
        public static PrimitiveTypeConverters Default { get; } = new PrimitiveTypeConverters();
        PrimitiveTypeConverters() {}

        public IEnumerator<ITypeConverter> GetEnumerator()
        {
            yield return BooleanTypeConverter.Default;
            yield return CharacterTypeConverter.Default;
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

    class AdditionalTypeConverters : ITypeConverters
    {
        public static AdditionalTypeConverters Default { get; } = new AdditionalTypeConverters();
        AdditionalTypeConverters() : this(Types.Default) {}

        private readonly ITypes _types;

        public AdditionalTypeConverters(ITypes types)
        {
            _types = types;
        }

        public IEnumerable<ITypeConverter> Get(IConverter parameter)
        {
            yield return new DictionaryTypeConverter(_types, parameter);
            yield return new ArrayTypeConverter(_types, parameter);
            yield return new EnumerableTypeConverter(_types, parameter);
            yield return new InstanceTypeConverter(_types, parameter);
        }

        class DictionaryTypeConverter : TypeConverter
        {
            public DictionaryTypeConverter(ITypes types, IConverter converter)
                : base(IsAssignableSpecification<IDictionary>.Default,
                       new DictionaryReader(types, converter), new DictionaryBodyWriter(converter)) {}
        }

        class EnumerableTypeConverter : TypeConverter
        {
            public EnumerableTypeConverter(ITypes types, IConverter converter)
                : base(
                    IsEnumerableTypeSpecification.Default,
                    new ListReader(types, converter),
                    new EnumerableBodyWriter(converter)
                ) {}
        }

        class InstanceTypeConverter : TypeConverter
        {
            public InstanceTypeConverter(ITypes types, IConverter converter)
                : this(
                    types, new InstanceMembers(new MemberFactory(converter, new EnumeratingReader(types, converter))),
                    Activators.Default) {}

            public InstanceTypeConverter(ITypes types, IInstanceMembers members, IActivators activators)
                : base(
                    IsActivatedTypeSpecification.Default, new InstanceBodyReader(members, types, activators),
                    new TypeEmittingWriter(new InstanceBodyWriter(members))) {}
        }
    }
}