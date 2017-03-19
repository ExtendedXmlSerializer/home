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

using System.Reflection;

namespace ExtendedXmlSerialization.Configuration
{
	public class ExtendedXmlMemberConfiguration<T> : IExtendedXmlMemberConfiguration
	{
		readonly IExtendedXmlMemberConfiguration _configuration;

		public ExtendedXmlMemberConfiguration(IExtendedXmlMemberConfiguration configuration,
		                                      ExtendedXmlTypeConfiguration<T> owner)
		{
			Owner = owner;
			_configuration = configuration;
		}

		public ExtendedXmlTypeConfiguration<T> Owner { get; }

		public MemberInfo Get() => _configuration.Get();

		public IExtendedXmlConfiguration Configuration => _configuration.Configuration;

		public IProperty<string> Name => _configuration.Name;

		IExtendedXmlTypeConfiguration IExtendedXmlMemberConfiguration.Owner => Owner;

		public IProperty<int> Order => _configuration.Order;
	}

	public class ExtendedXmlMemberConfiguration : IExtendedXmlMemberConfiguration
	{
		readonly MemberInfo _memberInfo;

		public ExtendedXmlMemberConfiguration(
			IExtendedXmlConfiguration configuration,
			IExtendedXmlTypeConfiguration owner, MemberInfo memberInfo,
			IProperty<string> name, IProperty<int> order)
		{
			Configuration = configuration;
			Owner = owner;
			Name = name;
			Order = order;
			_memberInfo = memberInfo;
		}

		public IExtendedXmlConfiguration Configuration { get; }
		public IExtendedXmlTypeConfiguration Owner { get; }

		public IProperty<string> Name { get; }
		public IProperty<int> Order { get; }

		public MemberInfo Get() => _memberInfo;
	}
}