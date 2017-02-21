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
using ExtendedXmlSerialization.ContentModel.Xml;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.ContentModel.Members
{
	class Member : Serializer, IMember
	{
		readonly ISpecification<object> _emit;
		readonly Action<object, object> _setter;
		readonly Func<object, object> _getter;

		public Member(MemberProfile profile, Func<object, object> getter, Action<object, object> setter)
			: this(profile.Specification, profile.Identity.Name, getter, setter, profile.Reader, profile.Writer) {}

		protected Member(ISpecification<object> emit, string displayName, Func<object, object> getter,
		                 Action<object, object> setter, IReader reader, IWriter writer) : base(reader, writer)
		{
			DisplayName = displayName;
			_emit = emit;
			_setter = setter;
			_getter = getter;
		}

		public override void Write(IXmlWriter writer, object instance)
		{
			var value = Get(instance);
			if (_emit.IsSatisfiedBy(value))
			{
				base.Write(writer, value);
			}
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