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

using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.References;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	sealed class AssignableStructureContents<T> : ConditionalContents<T>, IContents<T>
	{
		public AssignableStructureContents(AssignableStructures<T> source, IContents<T> fallback)
			: base(AssignableStructureSpecification.Default, source, fallback) {}
	}



	sealed class UnassignedContents<T> : ConditionalContents<T>, IContents<T>
	{
		public UnassignedContents(IContents<T> contents, IUnassignedContentReader<T> reader, IUnassignedContentWriter<T> writer)
			: base(AssignableStructureSpecification.Default.Or(IsReferenceSpecification.Default),
			       new UnassignedContentSerializers<T>(contents, reader, writer), contents) {}
	}

	sealed class UnassignedContentSerializers<T> : ISource<IContentSerializer<T>>
	{
		readonly IContents<T> _contents;
		readonly IContentReader<T> _reader;
		readonly IContentWriter<T> _writer;

		public UnassignedContentSerializers(IContents<T> contents, IContentReader<T> reader, IContentWriter<T> writer)
		{
			_contents = contents;
			_reader = reader;
			_writer = writer;
		}

		public IContentSerializer<T> Get()
		{
			var contents = _contents.Get();
			var result = new ContentSerializer<T>(new UnassignedContentAwareReader<T>(_reader, contents),
			                                      new UnassignedContentAwareWriter<T>(_writer, contents));
			return result;
		}
	}

	sealed class UnassignedContentAwareReader<T> : ConditionalContentReader<T>
	{
		public UnassignedContentAwareReader(IContentReader<T> @true, IContentReader<T> @false)
			: base(ContainsUnassignedContentSpecification.Default, @true, @false) {}
	}


	sealed class UnassignedContentAwareWriter<T> : ConditionalContentWriter<T>
	{
		public UnassignedContentAwareWriter(IContentWriter<T> @true, IContentWriter<T> @false) :
			this(AssignedSpecification<T>.Default.Inverse(), @true, @false) {}

		public UnassignedContentAwareWriter(ISpecification<T> specification, IContentWriter<T> @true,
		                                  IContentWriter<T> @false)
			: base(specification.To(InstanceCoercer<T>.Default), @true, @false) {}
	}

	sealed class InstanceCoercer<T> : IParameterizedSource<Writing<T>, T>
	{
		public static InstanceCoercer<T> Default { get; } = new InstanceCoercer<T>();
		InstanceCoercer() {}

		public T Get(Writing<T> parameter) => parameter.Instance;
	}

}