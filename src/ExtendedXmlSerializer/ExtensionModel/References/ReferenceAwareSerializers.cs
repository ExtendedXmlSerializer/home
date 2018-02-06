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

using System;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class ReferenceAwareSerializers : ContentModel.Content.ISerializers
	{
		readonly ISpecification<object> _conditions;
		readonly IStaticReferenceSpecification _specification;
		readonly IReferences _references;
		readonly ContentModel.Content.ISerializers _serializers;

		public ReferenceAwareSerializers(IStaticReferenceSpecification specification, IReferences references,
		                                 ContentModel.Content.ISerializers serializers)
			: this(new InstanceConditionalSpecification(), specification, references, serializers) {}

		public ReferenceAwareSerializers(ISpecification<object> conditions, IStaticReferenceSpecification specification,
		                                 IReferences references,
		                                 ContentModel.Content.ISerializers serializers)
		{
			_conditions = conditions;
			_specification = specification;
			_references = references;
			_serializers = serializers;
		}

		public ISerializer Get(TypeInfo parameter)
		{
			var serializer = _serializers.Get(parameter);
			var result = _specification.IsSatisfiedBy(parameter)
				             ? new Serializer(_conditions, _references, serializer)
				             : serializer;
			return result;
		}

		sealed class Serializer : ISerializer
		{
			readonly ISpecification<object> _conditions;
			readonly IReferences _references;
			readonly ISerializer _container;

			public Serializer(ISpecification<object> conditions, IReferences references, ISerializer container)
			{
				_conditions = conditions;
				_references = references;
				_container = container;
			}

			public object Get(IFormatReader parameter) => _container.Get(parameter);

			public void Write(IFormatWriter writer, object instance)
			{
				if (_conditions.IsSatisfiedBy(writer.Get()))
				{
					var references = _references.Get(instance);
					if (references.Any())
					{
						var typeInfo = instance.GetType()
						                       .GetTypeInfo();
						var line = Environment.NewLine;
						var message =
							$"{line}{line}Here is a list of found references:{line}{string.Join(line, references.Select(x => $"- {x}"))}";

						throw new CircularReferencesDetectedException(
						                                              $"The provided instance of type '{typeInfo}' contains circular references within its graph. Serializing this instance would result in a recursive, endless loop. To properly serialize this instance, please create a serializer that has referential support enabled by extending it with the ReferencesExtension.{message}",
						                                              _container);
					}
				}

				_container.Write(writer, instance);
			}
		}
	}
}