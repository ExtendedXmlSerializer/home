// MIT License
// 
// Copyright (c) 2016 Wojciech Nag�rski
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
using ExtendedXmlSerialization.Conversion.Elements;

namespace ExtendedXmlSerialization.Conversion.Members
{
	public class Member : Container, IMember
	{
		readonly Action<object, object> _setter;
		readonly Func<object, object> _getter;

		public Member(string displayName, Action<object, object> setter, Func<object, object> getter, IConverter body)
			: this(displayName, new StartMember(displayName), setter, getter, body) {}

		public Member(string displayName, IEmitter start, Action<object, object> setter, Func<object, object> getter,
		              IConverter body) : base(start, body)
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

/*
	public class Member : DecoratedConverter, IMember
	{
		readonly Action<object, object> _setter;
		readonly Func<object, object> _getter;
		readonly IElement _element;

		public Member(IElement element, Action<object, object> setter, Func<object, object> getter, IConverter body)
			: this(element.DisplayName, element.Classification, setter, getter, element, body) {}

		public Member(string displayName, TypeInfo classification, Action<object, object> setter,
		              Func<object, object> getter, IElement element, IConverter body) : base(body)
		{
			DisplayName = displayName;
			Classification = classification;
			_setter = setter;
			_getter = getter;
			_element = element;
		}

		public override void Emit(IWriter writer, object instance)
		{
			using (writer.Emit(_element, this))
			{
				base.Emit(writer, instance);
			}
		}

		public string DisplayName { get; }
		public TypeInfo Classification { get; }

		public virtual object Get(object instance) => _getter(instance);

		public virtual void Assign(object instance, object value)
		{
			if (value != null)
			{
				_setter(instance, value);
			}
		}
	}
*/
}