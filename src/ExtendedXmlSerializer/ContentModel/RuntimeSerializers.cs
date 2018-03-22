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
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

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
				             ? new Serializer(serializer, new Writer(parameter, _serializers, serializer))
				             : serializer;
			return result;
		}

		sealed class Writer : IWriter
		{
			readonly ISpecification<Type> _type;
			readonly ISerializers         _serializers;
			readonly ISerializer          _serializer;

			public Writer(Type type, ISerializers serializers, ISerializer serializer)
				: this(Reflection.VariableTypeSpecification.Defaults.Get(type), serializers, serializer) {}

			public Writer(ISpecification<Type> type, ISerializers serializers, ISerializer serializer)
			{
				_type        = type;
				_serializers = serializers;
				_serializer  = serializer;
			}

			public void Write(IFormatWriter writer, object instance)
			{
				var type     = instance?.GetType();
				var selected = type != null && _type.IsSatisfiedBy(type) ? _serializer : _serializers.Get(type);
				selected.Write(writer, instance);
			}
		}
	}
}