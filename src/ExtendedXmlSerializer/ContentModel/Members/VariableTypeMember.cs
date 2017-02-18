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
	class VariableTypeMember : IVariableTypeMember
	{
		readonly IMember _member;
		readonly ISpecification<Type> _specification;

		public VariableTypeMember(ISpecification<Type> specification, IMember member)
		{
			_member = member;
			_specification = specification;
		}

		public bool IsSatisfiedBy(Type parameter) => _specification.IsSatisfiedBy(parameter);

		public object Get(IXmlReader parameter) => ((IReader) _member).Get(parameter);

		public void Write(IXmlWriter writer, object instance) => _member.Write(writer, instance);

		public string DisplayName => _member.DisplayName;

		public object Get(object instance) => _member.Get(instance);

		public void Assign(object instance, object value) => _member.Assign(instance, value);
	}
}