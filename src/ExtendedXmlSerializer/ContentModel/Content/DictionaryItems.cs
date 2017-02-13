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

using System.Collections;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ContentModel.Content
{
	class DictionaryItems : IDictionaryItems
	{
		readonly static TypeInfo Type = typeof(DictionaryEntry).GetTypeInfo();
		readonly static PropertyInfo Key = Type.GetProperty(nameof(DictionaryEntry.Key));
		readonly static PropertyInfo Value = Type.GetProperty(nameof(DictionaryEntry.Value));
		readonly static IWriter Element = ElementOption.Default.Get(Type);

		readonly IMemberOption _variable;
		readonly IWriter _element;
		readonly IDictionaryPairTypesLocator _locator;

		public DictionaryItems(IMemberOption variable) : this(variable, Element, DictionaryPairTypesLocator.Default) {}

		public DictionaryItems(IMemberOption variable, IWriter element, IDictionaryPairTypesLocator locator)
		{
			_variable = variable;
			_element = element;
			_locator = locator;
		}

		IMember Create(MemberInfo metadata, TypeInfo classification)
			=> _variable.Get(new MemberInformation(metadata, classification, true));

		public ISerializer Get(TypeInfo parameter)
		{
			var pair = _locator.Get(parameter);
			var key = Create(Key, pair.KeyType);
			var value = Create(Value, pair.ValueType);
			var members = ImmutableArray.Create(key, value);
			var reader = new MemberedReader(Activator<DictionaryEntry>.Default, members.ToDictionary(x => x.DisplayName));
			var converter = new DecoratedSerializer(reader, new MemberWriter(members));
			var result = new Container(_element, converter);
			return result;
		}
	}
}