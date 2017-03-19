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
using System.Reflection;
using ExtendedXmlSerialization.ContentModel.Converters;

namespace ExtendedXmlSerialization.ContentModel.Members
{
	sealed class MemberConverters : IMemberConverters
	{
		readonly IMemberConverterSpecification _specification;
		readonly IConverters _converters;

		public MemberConverters(IMemberConverterSpecification specification, IConverters converters)
		{
			_specification = specification;
			_converters = converters;
		}

		public IConverter Get(MemberInfo parameter) => _specification.IsSatisfiedBy(parameter) ? From(parameter) : null;

		IConverter From(MemberDescriptor descriptor)
		{
			var result = _converters.Get(descriptor.MemberType);
			if (result != null)
			{
				return result;
			}
			throw new InvalidOperationException(
				$"An attempt was made to format '{descriptor.Metadata}' as an attribute, but there is not a registered converter that can convert its values to a string.  Please ensure a converter is registered for the type '{descriptor.MemberType}' by adding a converter for this type to the converter collection in the ConverterExtension.");
		}
	}
}