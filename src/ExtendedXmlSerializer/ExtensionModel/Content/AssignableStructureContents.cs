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

using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.References;
using ExtendedXmlSerializer.ReflectionModel;
using System.Reactive;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	sealed class AssignableStructureContents<T> : ConditionalContents<T>, IContents<T>
	{
		public AssignableStructureContents(AssignableStructures<T> source, IContents<T> fallback)
			: base(AssignableStructureSpecification.Default, source, fallback) {}
	}



	sealed class AssignableContents<T> : ConditionalContents<T>, IContents<T>
	{
		public AssignableContents(PropertyReference<EmitUnassignedSpecificationProperty, ISpecification<Unit>> property,
		                          IContents<T> contents, INullContentReader<T> reader, INullContentWriter<T> writer)
			: base(AssignableStructureSpecification.Default.Or(IsReferenceSpecification.Default),
			       new AssignableAwareContents<T>(contents, reader,
			                                      new ConditionalContentWriter<T>(property.Get().To(A<Writing<T>>.Default),
			                                                                      writer,
																				  EmptyContentWriter<T>.Default)
			                                      ), contents) {}
	}

	sealed class AssignableAwareContents<T> : ISource<IContentSerializer<T>>
	{
		readonly IContents<T> _contents;
		readonly IContentReader<T> _reader;
		readonly IContentWriter<T> _writer;

		public AssignableAwareContents(IContents<T> contents, IContentReader<T> reader, IContentWriter<T> writer)
		{
			_contents = contents;
			_reader = reader;
			_writer = writer;
		}

		public IContentSerializer<T> Get()
		{
			var contents = _contents.Get();
			var result = new ContentSerializer<T>(new AssignedContentAwareReader<T>(contents, _reader),
			                                      new AssignedContentAwareWriter<T>(contents, _writer));
			return result;
		}
	}
}