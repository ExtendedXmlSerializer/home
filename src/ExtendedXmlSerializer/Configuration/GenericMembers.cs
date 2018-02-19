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

using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.Configuration
{
	sealed class GenericMembers : IParameterizedSource<MemberInfo, IMemberConfiguration>
	{
		readonly ITypeConfiguration _element;
		readonly IGeneric<ITypeConfiguration, MemberInfo, IMemberConfiguration> _generic;
		readonly Func<MemberInfo, MemberDescriptor>                                _descriptor;
		readonly TypeInfo                                                          _type;

		public GenericMembers(ITypeConfiguration element) : this(element, element.Type()) {}

		public GenericMembers(ITypeConfiguration element, TypeInfo type)
			: this(element, Generic.Default, MemberDescriptors.Default.Get, type) {}

		public GenericMembers(ITypeConfiguration element,
		                      IGeneric<ITypeConfiguration, MemberInfo, IMemberConfiguration> generic,
		                      Func<MemberInfo, MemberDescriptor> descriptor, TypeInfo type)
		{
			_element    = element;
			_generic    = generic;
			_descriptor = descriptor;
			_type       = type;
		}

		public IMemberConfiguration Get(MemberInfo parameter) => _generic.Get(_type, _descriptor(parameter).MemberType)
		                                                                 .Invoke(_element, parameter);

		sealed class Generic : Generic<ITypeConfiguration, MemberInfo, IMemberConfiguration>
		{
			public static Generic Default { get; } = new Generic();
			Generic() : base(typeof(MemberConfiguration<,>)) {}
		}
	}
}