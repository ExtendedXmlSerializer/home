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
using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.ContentModel.Members
{
	public interface IMember : ISerializer
	{
		IMemberAdapter Adapter { get; }
	}

	public interface IMemberAdapter : IIdentity
	{
		MemberInfo Metadata { get; }

		TypeInfo MemberType { get; }

		bool IsWritable { get; }

		object Get(object instance);

		void Assign(object instance, object value);
	}

	class MemberAdapterSelector : IParameterizedSource<MemberProfile, IMemberAdapter>
	{
		readonly Func<object, object> _getter;
		readonly Action<object, object> _setter;

		public MemberAdapterSelector(Func<object, object> getter, Action<object, object> setter)
		{
			_getter = getter;
			_setter = setter;
		}

		public IMemberAdapter Get(MemberProfile parameter)
			=> new MemberAdapter(parameter.Specification, parameter.Identity.Name, parameter.Metadata,
			                     parameter.MemberType, parameter.AllowWrite, _getter, _setter);
	}

	sealed class MemberAdapter : Identity, IMemberAdapter
	{
		readonly ISpecification<object> _emit;
		readonly Func<object, object> _get;
		readonly Action<object, object> _set;

		public MemberAdapter(ISpecification<object> emit, string name, MemberInfo metadata, TypeInfo memberType,
		                     bool isWritable, Func<object, object> get, Action<object, object> set) : base(name, string.Empty)
		{
			_emit = emit;
			_get = get;
			_set = set;
			Metadata = metadata;
			MemberType = memberType;
			IsWritable = isWritable;
		}

		public MemberInfo Metadata { get; }
		public TypeInfo MemberType { get; }
		public bool IsWritable { get; }
		public object Get(object instance) => _get(instance);

		public void Assign(object instance, object value)
		{
			if (_emit.IsSatisfiedBy(value))
			{
				_set(instance, value);
			}
		}
	}
}