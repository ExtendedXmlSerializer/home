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

using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExtendedXmlSerializer.Configuration
{
	public static class Extensions
	{
		public static IRootContext Apply<T>(this IRootContext @this)
			where T : class, ISerializerExtension => Apply(@this, Support<T>.New);

		public static IRootContext Apply<T>(this IRootContext @this, Func<T> create)
			where T : class, ISerializerExtension
		{
			if (!@this.Contains<T>())
			{
				@this.Add(create);
			}
			return @this;
		}

		public static T Add<T>(this IRootContext @this) where T : ISerializerExtension
			=> Add(@this, Support<T>.New);

		public static T Add<T>(this IRootContext @this, Func<T> create) where T : ISerializerExtension
		{
			var result = create();
			@this.Add(result);
			return result;
		}

		public static IRootContext With<T>(this IRootContext @this, Action<T> configure)
			where T : class, ISerializerExtension
		{
			var extension = @this.With<T>();
			configure(extension);
			return @this;
		}

		public static T With<T>(this IRootContext @this) where T : class, ISerializerExtension
			=> @this.Find<T>() ?? @this.Add<T>();


		public static IRootContext Extend(this IRootContext @this,
		                                          params ISerializerExtension[] extensions)
		{
			var items = With(@this, extensions).ToList();
			@this.Clear();
			items.ForEach(@this.Add);
			return @this;
		}

		public static ISerializerExtension[] With(this IEnumerable<ISerializerExtension> @this,
		                                          params ISerializerExtension[] extensions)
			=> @this.TypeZip(extensions)
			        .ToArray();

		/*public static ITypeConfiguration Type(this IConfiguration @this, TypeInfo type) => @this.Get(type);*/

		public static TypeConfiguration<T> ConfigureType<T>(this IConfigurationContainer @this) => @this.Type<T>();

		public static TypeConfiguration<T> Type<T>(this IConfigurationContainer @this) => @this.Type(Support<T>.Key)
		                                                                              .AsValid<TypeConfiguration<T>>();

		public static ITypeConfiguration GetTypeConfiguration(this IConfigurationContainer @this, Type type)
			=> @this.GetTypeConfiguration(type.GetTypeInfo());

		public static ITypeConfiguration GetTypeConfiguration(this IConfigurationContainer @this, TypeInfo type) => @this.Type(type);

		public static MemberConfiguration<T, TMember> Member<T, TMember>(this TypeConfiguration<T> @this,
		                                                                 Expression<Func<T, TMember>> member) =>
			@this.Member(member.GetMemberInfo())
			     .AsValid<MemberConfiguration<T, TMember>>();

		public static IMemberConfiguration Member(this ITypeConfiguration @this, string member) => @this.Member(@this.Get()
		                                                                                                             .GetMember(member)
		                                                                                                             .Single());

		/*public static IConfigurationContainer EnableSingletons(this IConfigurationContainer @this) =>
			@this.Extend(SingletonActivationExtension.Default);*/
	}
}