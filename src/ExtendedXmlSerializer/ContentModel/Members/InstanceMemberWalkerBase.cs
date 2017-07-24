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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	abstract class InstanceMemberWalkerBase<T> : ObjectWalkerBase<object, IEnumerable<T>>
	{
		readonly ITypeMembers _members;
		readonly IEnumeratorStore _enumerators;
		readonly Func<object, IEnumerable<T>> _selector;

		protected InstanceMemberWalkerBase(ITypeMembers members, IEnumeratorStore enumerators, object root) : base(root)
		{
			_members = members;
			_enumerators = enumerators;
			_selector = Yield;
		}

		protected override IEnumerable<T> Select(object input)
		{
			var parameter = input.GetType().GetTypeInfo();
			var result = Members(input, parameter).Concat(Enumerate(input).Select(_selector).SelectMany(x => x));
			return result;
		}

		IEnumerable<object> Enumerate(object parameter)
		{
			var iterator = _enumerators.For(parameter);
			while (iterator?.MoveNext() ?? false)
			{
				yield return iterator.Current;
			}
		}

		protected virtual IEnumerable<T> Members(object input, TypeInfo parameter)
		{
			var members = _members.Get(parameter);
			var length = members.Length;

			for (var i = 0; i < length; i++)
			{
				var member = members[i];

				foreach (var item in Yield(member, input))
				{
					yield return item;
				}
			}
		}

		protected abstract IEnumerable<T> Yield(IMember member, object instance);

		protected abstract IEnumerable<T> Yield(object instance);
	}
}