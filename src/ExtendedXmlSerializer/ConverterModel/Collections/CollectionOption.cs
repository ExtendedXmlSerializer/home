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
using ExtendedXmlSerialization.ConverterModel.Converters;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ConverterModel.Collections
{
	abstract class CollectionOptionBase : ContentOptionBase
	{
		readonly IContainers _containers;
		readonly ICollectionItemTypeLocator _locator;

		protected CollectionOptionBase(IContainers containers) : this(IsCollectionTypeSpecification.Default, containers) {}

		protected CollectionOptionBase(ISpecification<TypeInfo> specification, IContainers containers)
			: this(specification, containers, CollectionItemTypeLocator.Default) {}

		protected CollectionOptionBase(ISpecification<TypeInfo> specification, IContainers containers,
		                               ICollectionItemTypeLocator locator) : base(specification)
		{
			_containers = containers;
			_locator = locator;
		}

		public override IConverter Get(TypeInfo parameter)
		{
			var itemType = _locator.Get(parameter);
			var result = Create(_containers.Get(itemType), itemType, parameter);
			return result;
		}

		protected abstract IConverter Create(IConverter item, TypeInfo itemType, TypeInfo classification);
	}

	class ArrayOption : CollectionOptionBase
	{
		public ArrayOption(IContainers container) : base(IsArraySpecification.Default, container) {}

		protected override IConverter Create(IConverter item, TypeInfo itemType, TypeInfo classification)
		{
			var reader = new ArrayReader(item);
			var result = new DecoratedConverter(reader, new EnumerableWriter(item));
			return result;
		}
	}

	class CollectionOption : CollectionOptionBase
	{
		readonly IActivators _activators;

		public CollectionOption(IContainers containers) : this(containers, Activators.Default) {}

		public CollectionOption(IContainers containers, IActivators activators)
			: base(containers)
		{
			_activators = activators;
		}

		protected override IConverter Create(IConverter item, TypeInfo itemType, TypeInfo classification)
		{
			var activator = new DelegatedFixedReader(_activators.Get(classification.AsType()));
			var reader = new CollectionReader(activator, item);
			var result = new DecoratedConverter(reader, new EnumerableWriter(item));
			return result;
		}
	}
}