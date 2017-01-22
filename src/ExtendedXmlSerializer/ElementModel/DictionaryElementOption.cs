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
using System.Reflection;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.ElementModel.Members;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ElementModel
{
	class DictionaryElementOption : MemberedCollectionElementOptionBase
	{
		readonly IDictionaryItemFactory _factory;

		public DictionaryElementOption(IElements elements, INames names, IElementMembers members)
			: this(elements, names, members, new DictionaryItemFactory(elements)) {}

		public DictionaryElementOption(IElements elements, INames names, IElementMembers members,
		                               IDictionaryItemFactory factory)
			: base(IsDictionaryTypeSpecification.Default, elements, names, members)
		{
			_factory = factory;
		}

		protected override IElement Create(string name, TypeInfo collectionType, IMembers members, Func<IElement> element)
		{
			var item = _factory.Get(collectionType);
			var result = new DictionaryElement(name, collectionType, item.Self, members);
			return result;
		}
	}

	public interface IDictionaryItemFactory : IParameterizedSource<TypeInfo, IElement> {}

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
			var result = new DictionaryItemElement(members);
			return result;
		}

		IMemberElement CreateMember(string name, TypeInfo type)
		{
			var memberInfo = DictionaryItemElement.Name.Classification.GetProperty(name);
			var setter = _setter.Get(memberInfo);
			var getter = _getter.Get(memberInfo);
			var result = new MemberElement(name, memberInfo, setter, getter, _elements.Build(type));
			return result;
		}
	}
}