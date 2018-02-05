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

using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class FormatWriters<T> : IFormatWriters<T>
	{
		readonly ISpecification<TypeInfo> _specification;
		readonly IFormatWriters<T> _writers;
		readonly IRootInstances _instances;

		public FormatWriters(IFormatWriters<T> writers, IRootInstances instances)
			: this(IsReferenceSpecification.Default, writers, instances) {}

		public FormatWriters(ISpecification<TypeInfo> specification, IFormatWriters<T> writers, IRootInstances instances)
		{
			_specification = specification;
			_writers = writers;
			_instances = instances;
		}

		public IFormatWriter Get(Writing<T> parameter)
		{
			var result = _writers.Get(parameter);
			var typeInfo = parameter.Instance.GetType()
			                        .GetTypeInfo();
			var isSatisfiedBy = _specification.IsSatisfiedBy(typeInfo);
			if (isSatisfiedBy)
			{
				_instances.Assign(result.Get(), parameter.Instance);
			}

			return result;
		}
	}
}