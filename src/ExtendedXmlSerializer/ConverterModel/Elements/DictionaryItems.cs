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
using ExtendedXmlSerialization.ConverterModel.Members;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ConverterModel.Elements
{
	class DictionaryItems : IDictionaryItems
	{
		readonly static TypeInfo Type = typeof(DictionaryEntry).GetTypeInfo();
		readonly static PropertyInfo Key = Type.GetProperty(nameof(DictionaryEntry.Key));
		readonly static PropertyInfo Value = Type.GetProperty(nameof(DictionaryEntry.Value));

		readonly IContainers _containers;
		readonly IWriter _element;
		readonly IGetterFactory _getter;
		readonly ISetterFactory _setter;
		readonly IDictionaryPairTypesLocator _locator;

		public DictionaryItems(IContainers containers)
			: this(
				containers, ElementOption.Default.Get(Type), GetterFactory.Default, SetterFactory.Default,
				DictionaryPairTypesLocator.Default
			) {}

		public DictionaryItems(IContainers containers, IWriter element, IGetterFactory getter,
		                       ISetterFactory setter, IDictionaryPairTypesLocator locator)
		{
			_containers = containers;
			_element = element;
			_getter = getter;
			_setter = setter;
			_locator = locator;
		}

		IMember Create(MemberInfo property, TypeInfo metadata)
			=>
				new VariableTypeMember(property.Name, metadata, _getter.Get(property), _setter.Get(property),
				                       _containers.Content(metadata));

		public IConverter Get(TypeInfo parameter)
		{
			var pair = _locator.Get(parameter);
			var key = Create(Key, pair.KeyType);
			var value = Create(Value, pair.ValueType);
			var members = ImmutableArray.Create(key, value);
			var reader = new MemberedReader(Activator<DictionaryEntry>.Default, members.ToDictionary(x => x.DisplayName));
			var converter = new DecoratedConverter(reader, new MemberWriter(members));
			var result = new Container(_element, converter);
			return result;
		}
	}
}