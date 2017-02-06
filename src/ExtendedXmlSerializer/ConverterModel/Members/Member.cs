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
using ExtendedXmlSerialization.ConverterModel.Elements;

namespace ExtendedXmlSerialization.ConverterModel.Members
{
	class Member : Container, IMember
	{
		readonly Action<object, object> _setter;
		readonly Func<object, object> _getter;

		public Member(string displayName, Action<object, object> setter, Func<object, object> getter, IConverter body)
			: this(displayName, new Elements.Member(displayName), setter, getter, body) {}

		public Member(string displayName, IWriter element, Action<object, object> setter, Func<object, object> getter,
		              IConverter body) : base(element, body)
		{
			DisplayName = displayName;
			_setter = setter;
			_getter = getter;
		}


		public string DisplayName { get; }

		public virtual object Get(object instance) => _getter(instance);

		public virtual void Assign(object instance, object value)
		{
			if (value != null)
			{
				_setter(instance, value);
			}
		}
	}
}