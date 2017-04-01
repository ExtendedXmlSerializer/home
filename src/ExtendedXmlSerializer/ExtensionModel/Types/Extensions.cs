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
using System.Linq.Expressions;
using System.Reflection;
using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	public static class Extensions
	{
		public static TypeConfiguration<T> ConfigureType<T>(this IConfiguration @this) => @this.Type<T>();

		public static TypeConfiguration<T> Type<T>(this IConfiguration @this)
			=> TypeConfigurations<T>.Default.Get(@this);

		public static MemberConfiguration<T, TMember> Member<T, TMember>(
			this TypeConfiguration<T> @this,
			Expression<Func<T, TMember>> member)
			=> Members<T, TMember>.Defaults.Get(@this.Configuration).Get(member.GetMemberInfo());

		public static TypeConfiguration<T> Member<T, TMember>(this TypeConfiguration<T> @this,
		                                                      Expression<Func<T, TMember>> member,
		                                                      Action<MemberConfiguration<T, TMember>>
			                                                      configure)
		{
			configure(@this.Member(member));
			return @this;
		}

		public static TypeConfiguration<T> Owner<T>(this IMemberConfiguration @this)
			=> TypeConfigurations<T>.Default.For(@this.Owner);

		public static string Name<T>(this IConfigurationItem<T> @this) where T : MemberInfo => @this.Name.Get();

		public static IConfigurationItem<T> Name<T>(this IConfigurationItem<T> @this, string name) where T : MemberInfo
		{
			@this.Name.Assign(name);
			return @this;
		}

		public static int Order(this IMemberConfiguration @this) => @this.Order.Get();

		public static IMemberConfiguration Order(this IMemberConfiguration @this, int order)
		{
			@this.Order.Assign(order);
			return @this;
		}

		public static ITypeConfiguration GetTypeConfiguration(this IConfiguration @this, Type type)
			=> @this.GetTypeConfiguration(type.GetTypeInfo());

		public static ITypeConfiguration GetTypeConfiguration(this IConfiguration @this, TypeInfo type)
			=> TypeConfigurations.Defaults.Get(@this).Get(type);

		public static IMemberConfiguration Member(this ITypeConfiguration @this, string name)
		{
			var member = @this.Get().GetMember(name).SingleOrDefault();
			var result = member != null ? @this.Member(member) : null;
			return result;
		}

		public static IConfiguration EnableSingletons(this IConfiguration @this)
			=> @this.Extend(SingletonActivationExtension.Default);
	}
}