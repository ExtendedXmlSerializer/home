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

using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class InstanceMemberSerialization : IInstanceMemberSerialization
	{
		readonly ISpecification<TypeInfo> _specification;
		readonly IMemberSerializations    _serializations;
		readonly IMemberSerialization     _serialization;

		public InstanceMemberSerialization(TypeInfo type, IMemberSerializations serializations)
			: this(VariableTypeSpecification.Defaults.Get(type), serializations, serializations.Get(type)) {}

		public InstanceMemberSerialization(ISpecification<TypeInfo> specification, IMemberSerializations serializations,
		                                   IMemberSerialization serialization)
		{
			_specification  = specification;
			_serializations = serializations;
			_serialization  = serialization;
		}

		public IMemberSerialization Get(object parameter)
		{
			var type = parameter.GetType()
			                    .GetTypeInfo();
			var result = _specification.IsSatisfiedBy(type) ? _serializations.Get(type) : _serialization;
			return result;
		}
	}
}