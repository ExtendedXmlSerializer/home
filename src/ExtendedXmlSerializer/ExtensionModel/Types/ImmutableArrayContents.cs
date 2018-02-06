// MIT License
// 
// Copyright (c) 2016-2018 Wojciech Nagórski
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

using System.Collections.Immutable;
using System.Collections.ObjectModel;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	sealed class ImmutableArrayContents : ICollectionContents
	{
		readonly IInnerContentServices _contents;
		readonly IEnumerators _enumerators;

		public ImmutableArrayContents(IInnerContentServices contents, IEnumerators enumerators)
		{
			_contents = contents;
			_enumerators = enumerators;
		}

		public ISerializer Get(CollectionContentInput parameter) => new Serializer(Readers.Default.Get(parameter.ItemType)
		                                                                                  .Invoke(_contents, parameter.Item),
		                                                                           new EnumerableWriter(_enumerators,
		                                                                                                parameter.Item)
			                                                                           .Adapt());

		sealed class Readers : Generic<IInnerContentServices, IReader, IReader>
		{
			public static Readers Default { get; } = new Readers();
			Readers() : base(typeof(Reader<>)) {}
		}

		sealed class Reader<T> : IReader
		{
			readonly ContentModel.IReader<Collection<T>> _reader;

			[UsedImplicitly]
			public Reader(IInnerContentServices services, IReader item)
				: this(services.CreateContents<Collection<T>>(new ConditionalInnerContentHandler(services,
				                                                                                 new
					                                                                                 CollectionInnerContentHandler(item,
					                                                                                                               services)))) {}

			Reader(ContentModel.IReader<Collection<T>> reader) => _reader = reader;

			public object Get(IFormatReader parameter) => _reader.Get(parameter)
			                                                     .ToImmutableArray();
		}
	}
}