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
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.References
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

			public object Get(IFormatReader parameter) => _container.Get(parameter);

			public void Write(IFormatWriter writer, object instance)
			{
				var references = _references.Get(writer);
				if (references.Any() && Conditions.Default.Get(writer).Apply()) // TODO: Might find a better way of handling this: https://github.com/wojtpl2/ExtendedXmlSerializer/issues/129
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
				_container.Write(writer, instance);
			}


		}
	}
}