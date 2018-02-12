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

using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.References;
using ExtendedXmlSerializer.ExtensionModel.Xml;
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
		public static IExtendedXmlSerializer Create(this IConfigurationContainer @this) => @this.Root.Create();

		public static bool Contains<T>(this IExtensionCollection @this) where T : ISerializerExtension
			=> @this.Any(IsTypeSpecification<T>.Default.IsSatisfiedBy);

		public static T Find<T>(this IExtensionCollection @this) where T : ISerializerExtension => @this.OfType<T>()
		                                                                                                .FirstOrDefault();

		/*public static T With<T>(this IRootContext @this) where T : class, ISerializerExtension
			=> @this.Find<T>() ?? Extend<T>.Default.Get(@this);*/


		/*public static IRootContext Extend(this IRootContext @this,
		                                  params ISerializerExtension[] extensions)
		{
			var items = With(@this, extensions)
				.ToList();
			@this.Clear();
			items.ForEach(@this.Add);
			return @this;
		}

		public static ISerializerExtension[] With(this IEnumerable<ISerializerExtension> @this,
		                                          params ISerializerExtension[] extensions)
			=> @this.TypeZip(extensions)
			        .ToArray();*/

		/*public static ITypeConfiguration Type(this IConfiguration @this, TypeInfo type) => @this.Get(type);*/

		public static IConfigurationContainer Configured<T>(this IConfigurationContainer @this) where T : class, IConfigurationProfile
			=> Support<T>.NewOrSingleton().Get(@this);

		public static ITypeConfiguration<T> ConfigureType<T>(this IConfigurationContainer @this) => @this.Type<T>();

		public static ITypeConfiguration<T> Type<T>(this IConfigurationContainer @this) => @this.Root.Types.Get(Support<T>.Key)
		                                                                              .AsValid<TypeConfiguration<T>>();

		public static IConfigurationContainer Type<T>(this IConfigurationContainer @this, Action<ITypeConfiguration<T>> configure)
		{
			var result = @this.Type<T>();
			configure(result);
			return @this;
		}

		public static ITypeConfiguration GetTypeConfiguration(this IConfigurationContainer @this, Type type)
			=> @this.GetTypeConfiguration(type.GetTypeInfo());

		public static ITypeConfiguration GetTypeConfiguration(this IConfigurationContainer @this, TypeInfo type) =>
			@this.Root.Types.Get(type);

		public static IMemberConfiguration<T, TMember> Member<T, TMember>(this ITypeConfiguration<T> @this,
		                                                                 Expression<Func<T, TMember>> member) =>
			((IInternalTypeConfiguration)@this).Member(member.GetMemberInfo())
				 .AsValid<MemberConfiguration<T, TMember>>();

		public static IMemberConfiguration<T, TMember> Name<T, TMember>(this IMemberConfiguration<T, TMember> @this, string name)
		{
			@this.AsInternal().Name(name);
			return @this;
		}

		public static IMemberConfiguration<T, TMember> Order<T, TMember>(this IMemberConfiguration<T, TMember> @this, int order)
		{
			@this.AsInternal().Order(order);
			return @this;
		}

		public static ITypeConfiguration<T> Name<T>(this ITypeConfiguration<T> @this, string name)
		{
			((IInternalTypeConfiguration)@this).Name(name);
			return @this;
		}

		public static ITypeConfiguration<T> Member<T>(this ITypeConfiguration<T> @this, MemberInfo member)
		{
			((IInternalTypeConfiguration)@this).Member(member);
			return @this;
		}

		internal static IMemberConfiguration Member(this ITypeConfiguration @this, string member)
		{
			var metadata = @this.Get()
			                    .GetMember(member)
			                    .SingleOrDefault();
			var result = metadata != null ? ((IInternalTypeConfiguration)@this).Member(metadata) : null;
			return result;
		}

		public static ITypeConfiguration<T> Member<T, TMember>(this ITypeConfiguration<T> @this,
		                                                      Expression<Func<T, TMember>> member,
		                                                      Action<IMemberConfiguration<T, TMember>> configure)
		{
			configure(@this.Member(member));
			return @this;
		}

		public static IConfigurationContainer EnableReferences(this IConfigurationContainer @this)
			=> @this.Extended<ReferencesExtension>();

		public static IConfigurationContainer EnableDeferredReferences(this IConfigurationContainer @this)
			=> @this.Extended<DeferredReferencesExtension>();

		public static ITypeConfiguration<T> EnableReferences<T, TMember>(this ITypeConfiguration<T> @this,
		                                                                Expression<Func<T, TMember>> member)
			=> @this.Member(member)
			        .Identity()
			        .Return(@this);

		public static IMemberConfiguration<T, TMember> Identity<T, TMember>(this IMemberConfiguration<T, TMember> @this)
			=> @this.Extend<ReferencesExtension>()
			        .Assigned(@this.Type(), @this.Member())
			        .Return(@this);

		public static ICollection<TypeInfo> AllowedReferenceTypes(this IConfigurationContainer @this)
			=> @this.Extend<DefaultReferencesExtension>()
			        .Whitelist;

		public static ICollection<TypeInfo> IgnoredReferenceTypes(this IConfigurationContainer @this)
			=> @this.Extend<DefaultReferencesExtension>()
			        .Blacklist;
	}
}