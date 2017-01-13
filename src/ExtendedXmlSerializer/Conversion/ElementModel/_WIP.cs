using System.Collections;
using System.Reflection;
using ExtendedXmlSerialization.Conversion.TypeModel;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion.ElementModel
{
    /*public interface IElementSelector : ISelector<TypeInfo, IElement> {}*/

    public interface IElementNameProvider : IParameterizedSource<MemberInfo, IElementName> {}

    class Elements : Selector<MemberInfo, IElement>, IElementFactory
    {
        public static Elements Default { get; } = new Elements();
        Elements() : this(DictionaryElementFactory.Default) {}

        public Elements(params IOption<MemberInfo, IElement>[] options) : base(options) {}
    }

    class DictionaryElementFactory : ElementOptionBase<TypeInfo>
    {
        public static DictionaryElementFactory Default { get; } = new DictionaryElementFactory();

        DictionaryElementFactory()
            : this(DictionaryPairTypesLocator.Default, EnumerableNameOption.Default, DefaultElementNameOption.Default) {}

        private readonly IDictionaryPairTypesLocator _locator;

        public DictionaryElementFactory(IDictionaryPairTypesLocator locator, params IElementNameOption[] names)
            : base(IsAssignableSpecification<IDictionary>.Default, names)
        {
            _locator = locator;
        }

        protected override IElement Create(TypeInfo parameter, IElementName name)
        {
            var pair = _locator.Get(parameter);
            var result = new DictionaryElement(name, pair.KeyType, pair.ValueType);
            return result;
        }
    }
}