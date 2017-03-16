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
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Configuration
{
	class ExtendedXmlTypeConfiguration : ReferenceCache<MemberInfo, IExtendedXmlMemberConfiguration>,
	                                     IExtendedXmlTypeConfiguration
	{
		readonly TypeInfo _type;

		public ExtendedXmlTypeConfiguration(TypeInfo type) : base(x => new ExtendedXmlMemberConfiguration(x))
		{
			_type = type;
		}

		public IExtendedXmlMemberConfiguration Member(string name) => Get(_type.GetMember(name).Single());

		public TypeInfo Get() => _type;
	}

	public class ExtendedXmlTypeConfiguration<T> : IExtendedXmlTypeConfiguration
	{
		readonly static TypeInfo Key = typeof(T).GetTypeInfo();

		readonly IExtendedXmlTypeConfiguration _configuration;

		public ExtendedXmlTypeConfiguration(IExtendedXmlConfiguration configuration)
			: this(configuration.GetTypeConfiguration(Key)) {}

		public ExtendedXmlTypeConfiguration(IExtendedXmlTypeConfiguration configuration)
		{
			var typeInfo = configuration.Get();
			if (!Key.IsAssignableFrom(typeInfo))
			{
				throw new InvalidOperationException($"{typeInfo} cannot be assigned to type '{Key}'");
			}

			_configuration = configuration;
		}

		public TypeInfo Get() => _configuration.Get();

		public IExtendedXmlMemberConfiguration Member(string name) => _configuration.Member(name);
	}
}