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
using System.Reflection;
using ExtendedXmlSerialization.Core;
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
			var result = new DictionaryElement(name, collectionType, new CollectionItem(item.Self), members);
			return result;
		}
	}
}