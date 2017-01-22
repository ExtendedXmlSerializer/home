using System.Collections;
using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.ElementModel.Members;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ElementModel
{
	public class DictionaryItemFactory : IDictionaryItemFactory
	{
		readonly IElements _elements;
		readonly IGetterFactory _getter;
		readonly ISetterFactory _setter;
		readonly IDictionaryPairTypesLocator _locator;

		public DictionaryItemFactory(IElements elements)
			: this(elements, GetterFactory.Default, SetterFactory.Default, DictionaryPairTypesLocator.Default) {}

		public DictionaryItemFactory(IElements elements, IGetterFactory getter, ISetterFactory setter,
		                             IDictionaryPairTypesLocator locator)
		{
			_elements = elements;
			_getter = getter;
			_setter = setter;
			_locator = locator;
		}

		public IElement Get(TypeInfo parameter)
		{
			var pair = _locator.Get(parameter);
			var members = new Members.Members(CreateMember(nameof(DictionaryEntry.Key), pair.KeyType),
			                                  CreateMember(nameof(DictionaryEntry.Value), pair.ValueType));
			var result = new DictionaryEntryElement(members);
			return result;
		}

		IMemberElement CreateMember(string name, TypeInfo type)
		{
			var memberInfo = DictionaryEntryElement.Name.Classification.GetProperty(name);
			var setter = _setter.Get(memberInfo);
			var getter = _getter.Get(memberInfo);
			var result = new MemberElement(name, memberInfo, setter, getter, _elements.Build(type));
			return result;
		}
	}
}