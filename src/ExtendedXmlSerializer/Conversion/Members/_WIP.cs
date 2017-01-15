using System.Reflection;
using ExtendedXmlSerialization.Conversion.ElementModel;
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.Conversion.Write;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion.Members
{
    public struct MemberInformation
    {
        public MemberInformation(MemberInfo metadata, TypeInfo memberType, bool assignable)
        {
            Metadata = metadata;
            MemberType = memberType;
            Assignable = assignable;
        }

        public MemberInfo Metadata { get; }
        public TypeInfo MemberType { get; }
        public bool Assignable { get; }
    }

    public interface IMemberOption : IOption<MemberInformation, IMemberElement> {}

    public abstract class MemberOptionBase : OptionBase<MemberInformation, IMemberElement>, IMemberOption
    {
        private readonly IElementNameProvider _provider;

        protected MemberOptionBase(ISpecification<MemberInformation> specification)
            : this(specification, MemberElementNameProvider.Default) {}

        protected MemberOptionBase(ISpecification<MemberInformation> specification, IElementNameProvider provider)
            : base(specification)
        {
            _provider = provider;
        }

        public override IMemberElement Get(MemberInformation parameter)
            => Create(parameter, _provider.Get(parameter.Metadata));

        protected abstract IMemberElement Create(MemberInformation parameter, IElementName name);
    }

    public interface IMemberConverterSelector : ISelector<IMemberElement, IConverter> {}

    class MemberConverterSelector : Selector<IMemberElement, IConverter>, IMemberConverterSelector
    {
        public MemberConverterSelector(IConverter converter) : base(
            new ReadOnlyCollectionMemberConverterOption(new Converter(new EnumeratingReader(converter), converter)),
            new MemberConverterOption(converter)
        ) {}

        protected override IConverter Create(IMemberElement parameter)
        {
            var result = base.Create(parameter);
            if (result == null)
            {
                // TODO: Warning? Throw?
            }
            return result;
        }
    }

    public class ReadOnlyCollectionMemberConverterOption : ConverterOptionBase<IReadOnlyCollectionMemberElement>
    {
        private readonly IConverter _converter;

        public ReadOnlyCollectionMemberConverterOption(IConverter converter)
        {
            _converter = converter;
        }

        protected override IConverter Create(IReadOnlyCollectionMemberElement parameter) => _converter;
    }

    public interface IMemberElementSelector : ISelector<MemberInformation, IMemberElement> {}

    class MemberElementSelector : OptionSelector<MemberInformation, IMemberElement>, IMemberElementSelector
    {
        public static MemberElementSelector Default { get; } = new MemberElementSelector();
        MemberElementSelector() : this(MemberOption.Default, ReadOnlyCollectionMemberOption.Default) {}

        public MemberElementSelector(params IOption<MemberInformation, IMemberElement>[] options) : base(options) {}
    }

    public class MemberOption : MemberOptionBase
    {
        public static MemberOption Default { get; } = new MemberOption();
        MemberOption() : this(GetterFactory.Default, SetterFactory.Default) {}

        private readonly IGetterFactory _getter;
        private readonly ISetterFactory _setter;

        public MemberOption(IGetterFactory getter, ISetterFactory setter)
            : base(new DelegatedSpecification<MemberInformation>(x => x.Assignable))
        {
            _getter = getter;
            _setter = setter;
        }

        protected override IMemberElement Create(MemberInformation parameter, IElementName name)
        {
            var getter = _getter.Get(parameter.Metadata);
            var setter = _setter.Get(parameter.Metadata);
            var result = new MemberElement(name, parameter.Metadata, parameter.MemberType, setter, getter);
            return result;
        }
    }
}