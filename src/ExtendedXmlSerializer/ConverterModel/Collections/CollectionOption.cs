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
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ConverterModel.Collections
{
	class CollectionOption : ConverterOptionBase
	{
		readonly IConverters _converters;
		readonly ICollectionItemTypeLocator _locator;
		readonly IWriters _writers;
		readonly IActivators _activators;
		readonly IAddDelegates _add;

		public CollectionOption(IConverters converters)
			: this(
				converters, CollectionItemTypeLocator.Default, Writers.Default, Activators.Default,
				AddDelegates.Default) {}

		public CollectionOption(IConverters converters, ICollectionItemTypeLocator locator, IWriters writers,
		                        IActivators activators, IAddDelegates add)
			: base(IsCollectionTypeSpecification.Default)
		{
			_converters = converters;
			_locator = locator;
			_writers = writers;
			_activators = activators;
			_add = add;
		}

		public override IConverter Get(TypeInfo parameter)
		{
			var item = _locator.Get(parameter);
			var context = new CollectionItem(_writers.Get(item), _converters.Get(item));
			var activator = new CollectionReader(new DelegatedFixedReader(_activators.Get(parameter.AsType())), context, _add);
			var result = new EnumerableConverter(context, activator);
			return result;
		}
	}
}