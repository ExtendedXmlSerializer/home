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

using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Xml;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel
{
	sealed class ReferenceAwareSerializers : CacheBase<TypeInfo, ISerializer>, ISerializers
	{
		readonly IStaticReferenceSpecification _specification;
		readonly IRootReferences _references;
		readonly ISerializers _serializers;

		public ReferenceAwareSerializers(IStaticReferenceSpecification specification, IRootReferences references,
		                                 ISerializers serializers)
		{
			_specification = specification;
			_references = references;
			_serializers = serializers;
		}

		protected override ISerializer Create(TypeInfo parameter)
		{
			var serializer = _serializers.Get(parameter);
			var result = _specification.IsSatisfiedBy(parameter) ? new Serializer(_references, serializer) : serializer;
			return result;
		}

		sealed class Serializer : ISerializer
		{
			readonly IRootReferences _references;
			readonly ISerializer _container;

			public Serializer(IRootReferences references, ISerializer container)
			{
				_references = references;
				_container = container;
			}

			public object Get(IXmlReader parameter) => _container.Get(parameter);

			public void Write(IXmlWriter writer, object instance)
			{
				var typeInfo = instance.GetType().GetTypeInfo();
				var readOnlyList = _references.Get(writer);
				if (readOnlyList.Any())
				{
					throw new CircularReferencesDetectedException(
						$"The provided instance of type '{typeInfo}' contains circular references within its graph. Serializing this instance would result in a recursive, endless loop. To properly serialize this instance, please create a serializer that has referential support enabled by extending it with the ReferencesExtension.",
						_container);
				}
				_container.Write(writer, instance);
			}
		}
	}
}