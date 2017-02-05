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
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ConverterModel.Collections
{
	abstract class CollectionOptionBase : ConverterOptionBase
	{
		readonly IConverters _converters;
		readonly IElements _elements;
		readonly ICollectionItemTypeLocator _locator;

		protected CollectionOptionBase(IConverters converters) : this(IsCollectionTypeSpecification.Default, converters) {}

		protected CollectionOptionBase(ISpecification<TypeInfo> specification, IConverters converters)
			: this(specification, converters, Elements.Default, CollectionItemTypeLocator.Default) {}

		protected CollectionOptionBase(ISpecification<TypeInfo> specification, IConverters converters, IElements elements,
		                               ICollectionItemTypeLocator locator) : base(specification)
		{
			_converters = converters;
			_elements = elements;
			_locator = locator;
		}

		public override IConverter Get(TypeInfo parameter)
		{
			var itemType = _locator.Get(parameter);
			var result = Create(_elements.Get(itemType), _converters.Get(itemType), itemType, parameter);
			return result;
		}

		protected abstract IConverter Create(IWriter element, IConverter body, TypeInfo itemType, TypeInfo parameter);
	}

	class ArrayOption : CollectionOptionBase
	{
		public ArrayOption(IConverters converters) : base(IsArraySpecification.Default, converters) {}

		protected override IConverter Create(IWriter element, IConverter body, TypeInfo itemType, TypeInfo parameter)
		{
			var item = new CollectionItem(element, body);
			var reader = new ArrayReader(item);
			var result = new EnumerableConverter(item, reader);
			return result;
		}
	}

	class CollectionOption : CollectionOptionBase
	{
		readonly IActivators _activators;

		public CollectionOption(IConverters converters) : this(converters, Activators.Default) {}

		public CollectionOption(IConverters converters, IActivators activators)
			: base(converters)
		{
			_activators = activators;
		}

		protected override IConverter Create(IWriter element, IConverter body, TypeInfo itemType, TypeInfo parameter)
		{
			var item = new CollectionItem(element, body);
			var activator = new DelegatedFixedReader(_activators.Get(parameter.AsType()));
			var reader = new CollectionReader(activator, item);
			var result = new EnumerableConverter(item, reader);
			return result;
		}
	}
}