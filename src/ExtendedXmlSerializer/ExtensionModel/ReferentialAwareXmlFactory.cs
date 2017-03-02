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

using System.IO;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.ContentModel.Xml;

namespace ExtendedXmlSerialization.ExtensionModel
{
	class ReferentialAwareXmlFactory : IXmlFactory
	{
		readonly IStaticReferenceSpecification _specification;
		readonly IRootReferences _references;
		readonly IXmlFactory _factory;

		public ReferentialAwareXmlFactory(IStaticReferenceSpecification specification, IRootReferences references,
		                                  IXmlFactory factory)
		{
			_specification = specification;
			_references = references;
			_factory = factory;
		}

		public IXmlWriter Create(Stream stream, object instance)
		{
			var result = _factory.Create(stream, instance);
			var typeInfo = instance.GetType().GetTypeInfo();
			var readOnlyList = _references.Get(result);
			if (_specification.IsSatisfiedBy(typeInfo) && readOnlyList.Any())
			{
				throw new CircularReferencesDetectedException(
					$"The provided instance of type '{typeInfo}' contains circular references within its graph. Serializing this instance would result in a recursive, endless loop. To properly serialize this instance, please create a serializer that has referential support enabled by extending it with the ReferencesExtension.",
					result);
			}
			return result;
		}

		public IXmlReader Create(Stream stream) => _factory.Create(stream);
	}
}