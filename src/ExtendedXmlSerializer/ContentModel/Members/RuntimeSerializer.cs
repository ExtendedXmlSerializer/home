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

using ExtendedXmlSerializer.ContentModel.Xml;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class RuntimeSerializer : IRuntimeSerializer
	{
		readonly ISpecification<object> _specification;
		readonly IMemberSerializer _property;
		readonly IMemberSerializer _content;

		public RuntimeSerializer(ISpecification<object> specification, IMemberSerializer property, IMemberSerializer content)
		{
			_specification = specification;
			_property = property;
			_content = content;
		}

		public IMemberSerializer Get(object parameter) => _specification.IsSatisfiedBy(parameter) ? _property : _content;

		public object Get(IXmlReader parameter) => _content.Get(parameter);

		public void Write(IXmlWriter writer, object instance) => _content.Write(writer, instance);
		public IMember Profile => _content.Profile;

		public IMemberAccess Access => _content.Access;
	}
}