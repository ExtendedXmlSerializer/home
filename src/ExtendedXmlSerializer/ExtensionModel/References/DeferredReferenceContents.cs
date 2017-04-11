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
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;
using IContents = ExtendedXmlSerializer.ContentModel.Content.IContents;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class DeferredReferenceContents : IContents
	{
		readonly static IsCollectionTypeSpecification IsCollectionTypeSpecification = IsCollectionTypeSpecification.Default;

		readonly ISpecification<TypeInfo> _specification;
		readonly IRootReferences _references;
		readonly IContents _contents;

		[UsedImplicitly]
		public DeferredReferenceContents(IRootReferences references, IContents contents)
			: this(IsCollectionTypeSpecification, references, contents) {}

		public DeferredReferenceContents(ISpecification<TypeInfo> specification, IRootReferences references,
		                                 IContents contents)
		{
			_specification = specification;
			_references = references;
			_contents = contents;
		}

		public ISerializer Get(TypeInfo parameter)
		{
			var serializer = _contents.Get(parameter);
			var result = serializer is ReferenceSerializer && _specification.IsSatisfiedBy(parameter)
				? new DeferredReferenceContent(_references, serializer)
				: serializer;
			return result;
		}
	}
}