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
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Configuration
{
	class ExtendedXmlTypeConfiguration : ReferenceCacheBase<MemberInfo, IExtendedXmlMemberConfiguration>,
	                                     IExtendedXmlTypeConfiguration
	{
		readonly TypeInfo _type;

		public ExtendedXmlTypeConfiguration(IExtendedXmlConfiguration configuration, IProperty<string> name, TypeInfo type)
		{
			Configuration = configuration;
			Name = name;
			_type = type;
		}

		public IExtendedXmlConfiguration Configuration { get; }
		public IProperty<string> Name { get; }

		protected override IExtendedXmlMemberConfiguration Create(MemberInfo parameter)
		{
			var extension = Configuration.With<MemberConfigurationExtension>();
			return new ExtendedXmlMemberConfiguration(Configuration, this, parameter,
			                                          new MemberProperty<string>(extension.Names, parameter),
			                                          new MemberProperty<int>(extension.Order, parameter)
			);
		}

		public IExtendedXmlMemberConfiguration Member(MemberInfo member) => Get(member);

		public TypeInfo Get() => _type;
	}

	public class ExtendedXmlTypeConfiguration<T> : IExtendedXmlTypeConfiguration
	{
		readonly static TypeInfo Key = Support<T>.Key;

		readonly IExtendedXmlTypeConfiguration _type;

		public ExtendedXmlTypeConfiguration(IExtendedXmlConfiguration configuration)
			: this(configuration, configuration.GetTypeConfiguration(Key)) {}

		public ExtendedXmlTypeConfiguration(IExtendedXmlConfiguration configuration, IExtendedXmlTypeConfiguration type)
		{
			Configuration = configuration;
			var typeInfo = type.Get();
			if (!Key.IsAssignableFrom(typeInfo))
			{
				throw new InvalidOperationException($"{typeInfo} cannot be assigned to type '{Key}'");
			}

			_type = type;
		}

		public IExtendedXmlConfiguration Configuration { get; }

		public TypeInfo Get() => _type.Get();

		public IProperty<string> Name => _type.Name;

		public IExtendedXmlMemberConfiguration Member(MemberInfo member) => _type.Member(member);
	}
}