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

using ExtendedXmlSerializer.Core.Sources;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	public class MemberTable<T> : TableSource<MemberInfo, T>, IMemberTable<T>
	{
		public MemberTable() : base(MemberComparer.Default) {}

		public MemberTable(IDictionary<MemberInfo, T> store) : base(store) {}
	}

	public class MetadataTable<T> : IMemberTable<T>
	{
		readonly ITableSource<TypeInfo, T> _types;
		readonly ITableSource<MemberInfo, T> _members;

		public MetadataTable() : this(new TypedTable<T>(), new MemberTable<T>()) {}

		public MetadataTable(ITableSource<TypeInfo, T> types, ITableSource<MemberInfo, T> members)
		{
			_types = types;
			_members = members;
		}

		public bool IsSatisfiedBy(MemberInfo parameter) => _types.IsSatisfiedBy(parameter) || _members.IsSatisfiedBy(parameter);

		public T Get(MemberInfo parameter)
		{
			if (parameter is TypeInfo type)
			{
				return _types.Get(type);
			}

			if (_members.IsSatisfiedBy(parameter))
			{
				return _members.Get(parameter);
			}

			var result = _members.IsSatisfiedBy(parameter.DeclaringType) ? _members.Get(parameter.DeclaringType) : default(T);
			return result;
		}

		public bool Remove(MemberInfo key)
		{
			if (key is TypeInfo type)
			{
				return _types.Remove(type);
			}

			if (_members.IsSatisfiedBy(key))
			{
				return _members.Remove(key);
			}

			var result = _members.IsSatisfiedBy(key.DeclaringType) && _members.Remove(key.DeclaringType);
			return result;
		}

		public void Execute(KeyValuePair<MemberInfo, T> parameter)
		{
			if (parameter.Key is TypeInfo type)
			{
				_types.Assign(type, parameter.Value);
			}
			else
			{
				_members.Assign(parameter.Key, parameter.Value);
			}
		}
	}
}