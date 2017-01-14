using System.Collections.Generic;
using System.Collections.Immutable;
using ExtendedXmlSerialization.Conversion.ElementModel;
using ExtendedXmlSerialization.Conversion.Members;
using ExtendedXmlSerialization.Conversion.Primitives;
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.Conversion.Write;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion
{
    public interface IConverterOption : IOption<IElement, IConverter> {}

    public abstract class ConverterOptionBase<T> : IConverterOption where T : IElement
    {
        private readonly ISpecification<IElement> _specification;

        protected ConverterOptionBase() : this(IsTypeSpecification<T>.Default) {}

        protected ConverterOptionBase(ISpecification<IElement> specification)
        {
            _specification = specification;
        }

        IConverter IParameterizedSource<IElement, IConverter>.Get(IElement parameter) => Create((T) parameter);

        protected abstract IConverter Create(T parameter);

        public bool IsSatisfiedBy(IElement parameter) => _specification.IsSatisfiedBy(parameter);
    }

    public class ConverterOption<T> : ConverterOptionBase<T> where T : IElement
    {
        private readonly IConverter _converter;
        public ConverterOption(IConverter converter)
        {
            _converter = converter;
        }

        protected override IConverter Create(T parameter) => _converter;
    }


    public class KnownConverters : ConverterOptionBase<IElement>
    {
        public static KnownConverters Default { get; } = new KnownConverters();

        KnownConverters() : this(
            BooleanTypeConverter.Default,
            CharacterTypeConverter.Default,
            ByteTypeConverter.Default,
            UnsignedByteTypeConverter.Default,
            ShortTypeConverter.Default,
            UnsignedShortTypeConverter.Default,
            IntegerTypeConverter.Default,
            UnsignedIntegerTypeConverter.Default,
            LongTypeConverter.Default,
            UnsignedLongTypeConverter.Default,
            FloatTypeConverter.Default,
            DoubleTypeConverter.Default,
            DecimalTypeConverter.Default,
            EnumerationTypeConverter.Default,
            DateTimeTypeConverter.Default,
            DateTimeOffsetTypeConverter.Default,
            StringTypeConverter.Default,
            GuidTypeConverter.Default,
            TimeSpanTypeConverter.Default
        ) {}

        private readonly ImmutableArray<ITypeConverter> _converters;

        public KnownConverters(params ITypeConverter[] converters) : this(converters.ToImmutableArray()) {}

        public KnownConverters(ImmutableArray<ITypeConverter> converters)
        {
            _converters = converters;
        }

        protected override IConverter Create(IElement parameter)
        {
            var type = parameter.Name.Classification;
            foreach (var converter in _converters)
            {
                if (converter.IsSatisfiedBy(type))
                {
                    return converter;
                }
            }
            return null;
        }
    }

    sealed class ConverterOptions : IParameterizedSource<IConverter, IEnumerable<IConverterOption>>
    {
        public static ConverterOptions Default { get; } = new ConverterOptions();
        ConverterOptions() {}

        public IEnumerable<IConverterOption> Get(IConverter parameter)
        {
            yield return KnownConverters.Default;
            yield return new ConverterOption<IDictionaryElement>(new DictionaryTypeConverter(parameter));
            yield return new ConverterOption<IArrayElement>(new ArrayTypeConverter(parameter));
            yield return new ConverterOption<ICollectionElement>(new EnumerableTypeConverter(parameter)); 
            yield return new ConverterOption<IActivatedElement>(new InstanceTypeConverter(parameter));
        }

        class DictionaryTypeConverter : Converter
        {
            public DictionaryTypeConverter(IConverter converter)
                : base(new DictionaryReader(converter), new DictionaryBodyWriter(converter)) {}
        }

        class EnumerableTypeConverter : Converter
        {
            public EnumerableTypeConverter(IConverter converter)
                : base(new ListReader(converter), new EnumerableBodyWriter(converter)) {}
        }

        class InstanceTypeConverter : Converter
        {
            public InstanceTypeConverter(IConverter converter)
                : this(ElementMembers.Default, new MemberConverterSelector(converter)) {}

            public InstanceTypeConverter(IElementMembers members, IMemberConverterSelector selector)
                : base(new InstanceBodyReader(members, selector),
                       new InstanceBodyWriter(members, selector)) {}
        }
    }

    public interface IConverterSelector : ISelector<IElement, IConverter> {}

    public class ConverterSelector : OptionSelector<IElement, IConverter>, IConverterSelector
    {
        public ConverterSelector(params IConverterOption[] options) : base(options) {}
    }
}