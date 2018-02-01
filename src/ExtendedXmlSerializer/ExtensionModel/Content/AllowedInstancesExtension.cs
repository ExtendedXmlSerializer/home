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
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	public sealed class AllowedInstancesExtension
		: TypedTable<ISpecification<object>>, ITypedSpecifications, ISerializerExtension
	{
		public AllowedInstancesExtension() :
			base(new Dictionary<TypeInfo, ISpecification<object>>(ReflectionModel.Defaults.TypeComparer)) {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterInstance<ITypedSpecifications>(this)
			            .Decorate<ISerializers, Serializers>()
			            .Decorate<IMemberAccessors, MemberAccessors>();

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class Serializers : ISerializers
		{
			readonly IParameterizedSource<TypeInfo, ISpecification<object>> _specifications;
			readonly ISerializers _serializers;

			public Serializers(ITypedSpecifications specifications, ISerializers serializers)
			{
				_specifications = specifications;
				_serializers = serializers;
			}

			public ISerializer Get(TypeInfo parameter)
			{
				var specification = _specifications.Get(parameter);
				var serializer = _serializers.Get(parameter);
				var result = specification != null ? new Serializer(specification, serializer) : serializer;
				return result;
			}
		}

		sealed class MemberAccessors : IMemberAccessors
		{
			readonly ITypedSpecifications _specifications;
			readonly IMemberAccessors _accessors;

			public MemberAccessors(ITypedSpecifications specifications, IMemberAccessors accessors)
			{
				_specifications = specifications;
				_accessors = accessors;
			}

			public IMemberAccess Get(IMember parameter)
			{
				var specification = _specifications.Get(parameter.MemberType);
				var access = _accessors.Get(parameter);
				var result = specification != null ? new MemberAccess(specification.And(access), access) : access;
				return result;
			}

			sealed class MemberAccess : IMemberAccess
			{
				readonly ISpecification<object> _specification;
				readonly IMemberAccess _access;

				public MemberAccess(ISpecification<object> specification, IMemberAccess access)
				{
					_specification = specification;
					_access = access;
				}

				public bool IsSatisfiedBy(object parameter) => _specification.IsSatisfiedBy(parameter);

				public object Get(object instance) => _access.Get(instance);

				public void Assign(object instance, object value)
				{
					_access.Assign(instance, value);
				}
			}
		}


		sealed class Serializer : ISerializer
		{
			readonly ISpecification<object> _specification;
			readonly ISerializer _serializer;

			public Serializer(ISpecification<object> specification, ISerializer serializer)
			{
				_specification = specification;
				_serializer = serializer;
			}

			public object Get(IFormatReader parameter) => _serializer.Get(parameter);

			public void Write(IFormatWriter writer, object instance)
			{
				if (_specification.IsSatisfiedBy(instance))
				{
					_serializer.Write(writer, instance);
				}
			}
		}
	}
}