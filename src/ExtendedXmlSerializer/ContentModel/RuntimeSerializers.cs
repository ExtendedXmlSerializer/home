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

using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel
{
	public sealed class RuntimeSerializers : ISerializers
	{
		readonly ISpecification<TypeInfo> _specification;
		readonly ISerializers             _serializers;

		public RuntimeSerializers(ISerializers serializers) : this(VariableTypeSpecification.Default, serializers) {}

		public RuntimeSerializers(ISpecification<TypeInfo> specification, ISerializers serializers)
		{
			_specification = specification;
			_serializers   = serializers;
		}

		public ISerializer Get(TypeInfo parameter)
		{
			var serializer = _serializers.Get(parameter);
			var result = _specification.IsSatisfiedBy(parameter)
				             ? new Serializer(serializer, new Writer(_serializers))
				             : serializer;
			return result;
		}

		sealed class Writer : IWriter
		{
			readonly ISerializers _serializers;

			public Writer(ISerializers serializers) => _serializers = serializers;

			public void Write(IFormatWriter writer, object instance)
			{
				_serializers.Get(instance.GetType())
				            .Write(writer, instance);
			}
		}
	}
}