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

using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class ParameterizedConstructorSpecification
		: AnySpecification<ConstructorInfo>, IValidConstructorSpecification
	{
		public ParameterizedConstructorSpecification(IValidConstructorSpecification specification,
		                                             IConstructorMembers source)
			: base(specification, source.IfAssigned()) {}
	}

	sealed class RuntimeSerializationExceptionMessage
		: DelegatedSource<TypeInfo, string>, IRuntimeSerializationExceptionMessage
	{
		public static IRuntimeSerializationExceptionMessage Default { get; }
			= new RuntimeSerializationExceptionMessage();

		RuntimeSerializationExceptionMessage() :
			base(x => @"Parameterized Content is enabled on the container.  By default, the type must satisfy the following rules if a public parameterless constructor is not found:

- Each member must not already be marked as an explicit contract
- Must be a public fields / property.
- Any public fields (spit) must be readonly
- Any public properties must have a get but not a set (on the public API, at least)
- There must be exactly one interesting constructor, with parameters that are a case-insensitive match for each field/property in some order (i.e. there must be an obvious 1:1 mapping between members and constructor parameter names)

More information can be found here: https://github.com/wojtpl2/ExtendedXmlSerializer/issues/222") {}
	}
}