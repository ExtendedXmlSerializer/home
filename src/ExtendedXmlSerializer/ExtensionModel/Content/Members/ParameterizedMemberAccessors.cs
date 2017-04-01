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

using System;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.TypeModel;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class ParameterizedMemberAccessors : IMemberAccessors
	{
		readonly IAllowedMemberValues _allow;
		readonly IMemberAccessors _accessors;
		readonly IGetterFactory _getter;

		public ParameterizedMemberAccessors(IAllowedMemberValues allow, IMemberAccessors accessors)
			: this(allow, accessors, GetterFactory.Default) {}

		public ParameterizedMemberAccessors(IAllowedMemberValues allow, IMemberAccessors accessors, IGetterFactory getter)
		{
			_allow = allow;
			_accessors = accessors;
			_getter = getter;
		}

		public IMemberAccess Get(IMember parameter)
			=>
				parameter is ParameterizedMember
					? new MemberAccess(_allow.Get(parameter.Metadata), _getter.Get(parameter.Metadata), parameter.Name)
					: Access(parameter);

		IMemberAccess Access(IMember parameter)
		{
			var access = _accessors.Get(parameter);
			var result = access != null ? new ActivatedMemberAccess(access, parameter.Name) : null;
			return result;
		}

		sealed class ActivatedMemberAccess : IMemberAccess
		{
			readonly IMemberAccess _member;
			readonly string _name;

			public ActivatedMemberAccess(IMemberAccess member, string name)
			{
				_member = member;
				_name = name;
			}

			public bool IsSatisfiedBy(object parameter) => _member.IsSatisfiedBy(parameter);

			public object Get(object instance) => (instance as IActivationContext)?.Get(_name) ?? _member.Get(instance);

			public void Assign(object instance, object value)
			{
				var context = instance as IActivationContext;
				if (context != null)
				{
					context.Assign(_name, value);
				}
				else
				{
					_member.Assign(instance, value);
				}
			}
		}

		sealed class MemberAccess : IMemberAccess
		{
			readonly ISpecification<object> _specification;
			readonly Func<object, object> _get;
			readonly string _name;

			public MemberAccess(ISpecification<object> specification, Func<object, object> get, string name)
			{
				_specification = specification;
				_get = get;
				_name = name;
			}

			public bool IsSatisfiedBy(object parameter) => _specification.IsSatisfiedBy(parameter);

			public object Get(object instance) => _get.Invoke(instance);

			public void Assign(object instance, object value) => (instance as IActivationContext)?.Assign(_name, value);
		}
	}
}