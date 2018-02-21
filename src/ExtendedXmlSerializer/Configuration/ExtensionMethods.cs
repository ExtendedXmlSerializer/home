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
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Content.Members;
using ExtendedXmlSerializer.ExtensionModel.References;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Properties = ExtendedXmlSerializer.ExtensionModel.Properties;

namespace ExtendedXmlSerializer.Configuration
{
	public static class ExtensionMethods
	{
		public static IExtendedXmlSerializer Create(this IConfigurationElement @this)
			=> @this.Create<IExtendedXmlSerializer>();

		public static T Create<T>(this IConfigurationElement @this) => @this.Service<Func<T>>().Invoke();

		public static T Extend<T>(this IExtend @this) where T : class, ISerializerExtension
			=> @this.Get(typeof(T)).To<T>();

		public static T Extend<T>(this IConfigurationElement @this) where T : class, ISerializerExtension
			=> @this.Extensions.Extend<T>();

		public static IConfigurationElement Extended<T>(this IConfigurationElement @this)
			where T : class, ISerializerExtension
			=> @this.Extensions.Extend<T>().Return(@this);

		public static T Service<T>(this IConfigurationElement @this) => @this.Extensions.Service<T>();

		public static T Service<T>(this IExtensions @this) => @this.Extend<ConfigurationServicesExtension>().Get<T>();

		public static THost Set<THost, T>(this THost @this, IProperty<THost, T> property, T value)
			where THost : class, IConfigurationElement => @this.Entry(property).Executed(value).Return(@this);

		public static T Get<THost, T>(this THost @this, IProperty<THost, T> property)
			where THost : class, IConfigurationElement => @this.Entry(property).Get();

		public static IEntry<T> Entry<THost, T>(this THost @this, IProperty<THost, T> property)
			where THost : class, IConfigurationElement
			=> TableEntries<THost, T>.Defaults
			                         .Get(Properties.Defaults.Get(@this.Extensions).ReturnWith(property))
			                         .Get(@this);


		public static TSource Set<TSource, THost, T>(this TSource @this, IProperty<ISource<THost>, T> property, T value)
			where TSource : class, IConfigurationElement, ISource<THost>
			where THost : class
			=> @this.Entry(property).Executed(value).Return(@this);

		public static T Get<TSource, THost, T>(this TSource @this, IProperty<ISource<THost>, T> property)
			where TSource : class, IConfigurationElement, ISource<THost>
			where THost : class
			=> @this.Entry(property).Get();


		public static IEntry<T> Entry<TSource, THost, T>(this TSource @this, IProperty<ISource<THost>, T> property)
			where TSource : class, IConfigurationElement, ISource<THost>
			where THost : class
			=> TableEntries<ISource<THost>, T>.Defaults
			                         .Get(Properties.Defaults.Get(@this.Extensions).ReturnWith(property))
			                         .Get(@this);

		public static T Service<T>(this T @this, object service) where T : class, IConfigurationElement
			=> @this.Extend<ConfigurationServicesExtension>()
			        .Executed(service)
			        .Return(@this);

		public static T Services<T>(this T @this, params object[] services) where T : class, IConfigurationElement
		{
			var extension = @this.Extend<ConfigurationServicesExtension>();
			services.ForEach(extension.Execute);
			return @this;
		}

		public static IConfigurationElement Configured<T>(this IConfigurationElement @this)
			where T : class, IConfigurationProfile => Support<T>.NewOrSingleton()
			                                                    .Get(@this);

		public static IEnumerable<ITypeConfiguration> Types(this IConfigurationElement @this)
			=> @this.Service<IConfiguredTypes>();

		public static IType<T> ConfigureType<T>(this IConfigurationElement @this) => @this.Type<T>();

		public static IType<T> Type<T>(this IConfigurationElement @this)
			=> @this.GetTypeConfiguration(Support<T>.Key)
			        .To<IType<T>>();

		public static IConfigurationElement Type<T>(this IConfigurationElement @this,
		                                            Action<IType<T>> configure)
		{
			var result = @this.Type<T>();
			configure(result);
			return @this;
		}

		public static ITypeConfiguration GetTypeConfiguration(this IConfigurationElement @this, Type type)
			=> @this.GetTypeConfiguration(type.GetTypeInfo());

		public static ITypeConfiguration GetTypeConfiguration(this IConfigurationElement @this, TypeInfo type) =>
			@this.Service<IConfiguredTypes>()
			     .Get(type);

		public static MemberConfiguration<T, TMember> Order<T, TMember>(this MemberConfiguration<T, TMember> @this,
		                                                                int order)
			=> @this.Extend<MemberOrderingExtension>()
			        .Assign(@this.Member(), order)
			        .Return(@this);

		public static T Name<T>(this T @this, string name) where T : class, IMetadataConfiguration
			=> @this.Set(RegisteredNamesProperty.Default, name);

		public static IMemberConfiguration Member(this ITypeConfiguration @this, string member)
			=> @this.Member(@this.Type()
			                     .GetMember(member)
			                     .Single());

		public static IMemberConfiguration Member(this ITypeConfiguration @this, MemberInfo member)
			=> @this.Members().Get(member);

		public static MemberConfiguration<T, TMember> Member<T, TMember>(this IType<T> @this,
		                                                                 Expression<Func<T, TMember>> member)
			=> @this.Member(member.GetMemberInfo()).To<MemberConfiguration<T, TMember>>();

		public static IMembers Members(this ITypeConfiguration @this) => ExtendedXmlSerializer.Configuration.Members.Default.Get(@this);

		public static IType<T> Member<T, TMember>(this IType<T> @this,
		                                          Expression<Func<T, TMember>> member,
		                                          Action<MemberConfiguration<T, TMember>> configure)
		{
			configure(@this.Member(member));
			return @this;
		}

		public static IConfigurationElement EnableReferences(this IConfigurationElement @this)
			=> @this.Extended<ReferencesExtension>();

		public static IConfigurationElement EnableDeferredReferences(this IConfigurationElement @this)
			=> @this.Extended<DeferredReferencesExtension>();

		public static IType<T> EnableReferences<T, TMember>(this IType<T> @this,
		                                                    Expression<Func<T, TMember>> member)
			=> @this.Member(member)
			        .Identity()
			        .Return(@this);

		public static MemberConfiguration<T, TMember> Identity<T, TMember>(this MemberConfiguration<T, TMember> @this)
			=> @this.Extend<ReferencesExtension>()
			        .Assign(@this.Type(), @this.Member())
			        .Return(@this);

		public static ICollection<TypeInfo> AllowedReferenceTypes(this IConfigurationElement @this)
			=> @this.Extend<DefaultReferencesExtension>()
			        .Whitelist;

		public static ICollection<TypeInfo> IgnoredReferenceTypes(this IConfigurationElement @this)
			=> @this.Extend<DefaultReferencesExtension>()
			        .Blacklist;
	}
}