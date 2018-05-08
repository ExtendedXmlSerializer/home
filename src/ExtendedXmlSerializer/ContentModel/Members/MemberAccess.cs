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
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberAccess : DecoratedSpecification<object>, IMemberAccess
	{
		public ISpecification<object> Instance { get; }
		readonly Func<object, object>   _get;
		readonly Action<object, object> _set;

		public MemberAccess(ISpecification<object> emit, Func<object, object> get, Action<object, object> set)
			: this(emit, emit.GetInstance(), get, set) {}

		public MemberAccess(ISpecification<object> emit, ISpecification<object> instance, Func<object, object> get,
		                    Action<object, object> set) : base(emit)
		{
			Instance = instance;
			_get     = get;
			_set     = set;
		}

		public object Get(object instance) => instance != null ? _get(instance) : null;

		public void Assign(object instance, object value)
		{
			if (IsSatisfiedBy(value))
			{
				_set(instance, value);
			}
		}
	}
}