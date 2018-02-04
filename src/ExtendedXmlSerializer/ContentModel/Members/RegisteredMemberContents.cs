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
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class RegisteredMemberContents : IMemberContents, ISpecification<IMember>
	{
		readonly IActivators _activators;
		readonly ISpecification<MemberInfo> _specification;
		readonly IParameterizedSource<MemberInfo, ISerializer> _serializers;

		public RegisteredMemberContents(IActivators activators, ICustomMemberSerializers serializers)
			: this(activators, serializers.Or(IsDefinedSpecification<ContentSerializerAttribute>.Default), serializers) {}

		public RegisteredMemberContents(IActivators activators, ISpecification<MemberInfo> specification,
		                                IParameterizedSource<MemberInfo, ISerializer> serializers)
		{
			_activators = activators;
			_specification = specification;
			_serializers = serializers;
		}

		public bool IsSatisfiedBy(IMember parameter) => _specification.IsSatisfiedBy(parameter.Metadata);

		public ISerializer Get(IMember parameter) => _serializers.Get(parameter.Metadata) ?? Activate(parameter);

		ISerializer Activate(IMember parameter)
		{
			var typeInfo = parameter.Metadata.GetCustomAttribute<ContentSerializerAttribute>()
			                        .SerializerType.GetTypeInfo();
			var instance = _activators.Get(typeInfo)
			                          .Get();
			var result = instance as ISerializer ?? GenericSerializers.Default.Get(parameter.MemberType)
			                                                          .Invoke(instance);
			return result;
		}
	}
}