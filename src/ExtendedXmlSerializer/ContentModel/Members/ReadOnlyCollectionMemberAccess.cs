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

using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.Core.Specifications;
using System.Collections;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	class ReadOnlyCollectionMemberAccess : DecoratedSpecification<object>, IMemberAccess
	{
		readonly IMemberAccess _access;

		public ReadOnlyCollectionMemberAccess(IMemberAccess access) : base(access) => _access = access;

		public object Get(object instance)
		{
			var current = _access.Get(instance);
			var list = Lists.Default.Get(current);
			var result = list.Count > 0 ? current : null;
			return result;
		}

		public void Assign(object instance, object value)
		{
			var collection = _access.Get(instance);
			foreach (var element in (IEnumerable) value)
			{
				_access.Assign(collection, element);
			}
		}
	}

	class ReadOnlyCollectionMemberAccess<T, TMember> : DecoratedSpecification<TMember>, IMemberAccess<T, TMember>
	{
		readonly IMemberAccess<T, TMember> _access;

		public ReadOnlyCollectionMemberAccess(IMemberAccess<T, TMember> access) : base(access) => _access = access;

		public TMember Get(T instance)
		{
			var current = _access.Get(instance);
			var list = Lists.Default.Get(current);
			var result = list.Count > 0 ? current : default(TMember);
			return result;
		}

		public void Assign(T instance, TMember value)
		{
/*
			var collection = _access.Get(instance);
			foreach (var element in value)
			{
				_access.Assign(collection, element);
			}
*/
		}

		public void Execute(KeyValuePair<T, TMember> parameter)
		{
			throw new System.NotImplementedException();
		}
	}

}