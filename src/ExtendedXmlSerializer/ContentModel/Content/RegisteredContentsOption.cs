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
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class RegisteredContentsOption : IContentOption
	{
		readonly IActivators _activators;
		readonly ISpecification<TypeInfo> _specification;
		readonly ITypedTable<ISerializer> _serializers;

		public RegisteredContentsOption(IActivators activators, ICustomSerializers serializers)
			: this(activators, IsDefinedSpecification<ContentSerializerAttribute>.Default.Or(serializers), serializers) {}

		public RegisteredContentsOption(IActivators activators, ISpecification<TypeInfo> specification,
		                                ITypedTable<ISerializer> serializers)
		{
			_activators = activators;
			_specification = specification;
			_serializers = serializers;
		}

		public bool IsSatisfiedBy(TypeInfo parameter) => _specification.IsSatisfiedBy(parameter);

		public ISerializer Get(TypeInfo parameter) => _serializers.Get(parameter) ?? Activate(parameter);

		ISerializer Activate(TypeInfo parameter)
		{
			var typeInfo = parameter.GetCustomAttribute<ContentSerializerAttribute>()
			                        .SerializerType.GetTypeInfo();
			var instance = _activators.Get(typeInfo)
			                          .Get();
			var result = instance as ISerializer ?? GenericSerializers.Default.Get(parameter)
			                                                          .Invoke(instance);
			return result;
		}
	}
}