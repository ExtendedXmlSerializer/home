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
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.Configuration
{
	sealed class Members : IParameterizedSource<MemberInfo, IMemberConfiguration>
	{
		readonly ITypeConfiguration _type;
		readonly IGeneric<ITypeConfiguration, MemberInfo, IMemberConfiguration> _generic;
		readonly Func<MemberInfo, MemberDescriptor> _descriptor;

		public Members(ITypeConfiguration type) : this(type, Generic.Default, MemberDescriptors.Default.Get) {}

		public Members(ITypeConfiguration type, IGeneric<ITypeConfiguration, MemberInfo, IMemberConfiguration> generic,
		               Func<MemberInfo, MemberDescriptor> descriptor)
		{
			_type = type;
			_generic = generic;
			_descriptor = descriptor;
		}

		public IMemberConfiguration Get(MemberInfo parameter) => _generic.Get(_type.Get(), _descriptor(parameter)
			                                                                      .MemberType)
		                                                                 .Invoke(_type, parameter);

		sealed class Generic : Generic<IConfigurationElement, MemberInfo, IMemberConfiguration>
		{
			public static Generic Default { get; } = new Generic();
			Generic() : base(typeof(MemberConfiguration<,>)) {}
		}
	}
}