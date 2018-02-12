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
using ExtendedXmlSerializer.ReflectionModel;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	class ConditionalElements<T> : ConditionalSource<IContentWriter<T>>, IElements<T>
	{
		public ConditionalElements(ISpecification<TypeInfo> specification, IElements<T> source, IElements<T> fallback)
			: base(specification.Fix(Support<T>.Key), source, fallback) {}
	}

	class ConditionalContents<T> : ConditionalSource<IContentSerializer<T>>
	{
		public ConditionalContents(ISpecificationSource<TypeInfo, IContentSerializer<T>> source, ISource<IContentSerializer<T>> fallback)
			: this(source, source, fallback) {}

		public ConditionalContents(ISpecification<TypeInfo> specification, IParameterizedSource<TypeInfo, IContentSerializer<T>> source, ISource<IContentSerializer<T>> fallback)
			: this(specification, source.Fix(Support<T>.Key), fallback) {}

		public ConditionalContents(ISpecification<TypeInfo> specification, ISource<IContentSerializer<T>> source, ISource<IContentSerializer<T>> fallback)
			: base(specification.Fix(Support<T>.Key), source, fallback) {}
	}
}