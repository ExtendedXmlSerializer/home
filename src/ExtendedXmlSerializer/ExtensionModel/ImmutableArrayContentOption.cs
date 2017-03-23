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
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Xml;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.TypeModel;

namespace ExtendedXmlSerializer.ExtensionModel
{
	sealed class ImmutableArrayContentOption : CollectionContentOptionBase
	{
		readonly static ISpecification<TypeInfo> Specification = new IsAssignableGenericSpecification(typeof(ImmutableArray<>));

		readonly IActivation _activation;
		readonly IEnumerators _enumerators;

		public ImmutableArrayContentOption(IActivation activation, IEnumerators enumerators, ISerializers serializers)
			: base(Specification, serializers)
		{
			_activation = activation;
			_enumerators = enumerators;
		}

		protected override ISerializer Create(ISerializer item, TypeInfo classification, TypeInfo itemType)
			=> new Serializer(CreateReader(_activation, item, itemType), new EnumerableWriter(_enumerators, item));

		static IReader CreateReader(IActivation activation, ISerializer item, TypeInfo itemType)
			=> (IReader) Activator.CreateInstance(typeof(Reader<>).MakeGenericType(itemType.AsType()), activation, item);

		sealed class Reader<T> : DecoratedReader
		{
			public Reader(IActivation activation, IReader item)
				: base(new CollectionContentsReader(activation.Get<Collection<T>>(), item)) {}

			public override object Get(IXmlReader parameter) => base.Get(parameter)
			                                                        .AsValid<Collection<T>>()
			                                                        .ToImmutableArray();
		}
	}
}