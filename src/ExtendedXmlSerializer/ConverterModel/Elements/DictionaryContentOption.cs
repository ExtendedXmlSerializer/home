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

using System.Reflection;
using ExtendedXmlSerialization.ConverterModel.Collections;
using ExtendedXmlSerialization.ConverterModel.Members;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ConverterModel.Elements
{
	class DictionaryContentOption : ContentOptionBase
	{
		readonly IDictionaryItems _items;
		readonly IActivators _activators;

		public DictionaryContentOption(IMemberOption variable)
			: this(new DictionaryItems(variable), Activators.Default) {}

		public DictionaryContentOption(IDictionaryItems items, IActivators activators)
			: base(IsDictionaryTypeSpecification.Default)
		{
			_items = items;
			_activators = activators;
		}

		public override IConverter Get(TypeInfo parameter)
		{
			var item = _items.Get(parameter);
			var activator = new DelegatedFixedActivator(_activators.Get(parameter.AsType()));
			var reader = new CollectionReader(activator, item, DictionaryAddDelegates.Default);
			var result = new DecoratedConverter(reader, new DictionaryWriter(item));
			return result;
		}
	}
}