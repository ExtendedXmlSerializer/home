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
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExtendedXmlSerializer.Configuration
{
	public static class ExtensionMethods
	{
		public static T Extend<T>(this IExtend @this) where T : class, ISerializerExtension
			=> @this.Get(typeof(T))
			        .To<T>();

		public static IConfigurationElement Extended<T>(this IConfigurationElement @this)
			where T : class, ISerializerExtension
			=> @this.Extend<T>()
			        .Return(@this);


		public static T Service<T>(this IExtend @this) => @this.Extend<ConfigurationServicesExtension>()
		                                                       .Get<T>();


		public static T Find<T>(this IExtensions @this) where T : ISerializerExtension => @this.OfType<T>()
		                                                                                       .FirstOrDefault();


		public static IConfigurationElement Configured<T>(this IConfigurationElement @this)
			where T : class, IConfigurationProfile
			=> Support<T>.NewOrSingleton()
			             .Get(@this);

		public static IEnumerable<ITypeConfiguration> Types(this IConfigurationElement @this)
			=> @this.Service<IMetadataConfigurations>();

		public static TypeConfiguration<T> ConfigureType<T>(this IConfigurationElement @this) => @this.Type<T>();

		public static TypeConfiguration<T> Type<T>(this IConfigurationElement @this)
			=> @this.GetTypeConfiguration(Support<T>.Key)
			        .To<TypeConfiguration<T>>();

		public static IConfigurationElement Type<T>(this IConfigurationElement @this,
		                                            Action<TypeConfiguration<T>> configure)
		{
			var result = @this.Type<T>();
			configure(result);
			return @this;
		}

		public static ITypeConfiguration GetTypeConfiguration(this IConfigurationElement @this, Type type)
			=> @this.GetTypeConfiguration(type.GetTypeInfo());

		public static ITypeConfiguration GetTypeConfiguration(this IConfigurationElement @this, TypeInfo type) =>
			@this.Service<IMetadataConfigurations>()
			     .Get(type);

		public static MemberConfiguration<T, TMember> Name<T, TMember>(this MemberConfiguration<T, TMember> @this,
		                                                                string name)
			=> @this.Extend<MemberNamesExtension>()
			        .Assigned(@this.Member(), name)
			        .Return(@this);

		public static MemberConfiguration<T, TMember> Order<T, TMember>(this MemberConfiguration<T, TMember> @this,
		                                                                 int order)
			=> @this.Extend<MemberOrderingExtension>()
			        .Assigned(@this.Member(), order)
			        .Return(@this);

		public static TypeConfiguration<T> Name<T>(this TypeConfiguration<T> @this, string name)
			=> @this.Extend<MemberNamesExtension>()
			        .Assigned(@this.Type(), name)
			        .Return(@this);

		public static IMemberConfiguration Member(this ITypeConfiguration @this, string member)
		{
			var metadata = @this.Type()
			                    .GetMember(member)
			                    .SingleOrDefault();
			var result = metadata != null
				             ? @this.Member(metadata)
				             : null;
			return result;
		}

		public static IMemberConfiguration Member(this ITypeConfiguration @this, MemberInfo member)
			=> @this.Members()
			        .Get(member);

		public static MemberConfiguration<T, TMember> Member<T, TMember>(this TypeConfiguration<T> @this,
		                                                                  Expression<Func<T, TMember>> member)
			=> @this.Member(member.GetMemberInfo())
			        .To<MemberConfiguration<T, TMember>>();

		public static IMemberConfigurations Members(this ITypeConfiguration @this)
			=> @this.Service<IMetadataConfigurations>()
			        .Get(@this);

		public static TypeConfiguration<T> Member<T, TMember>(this TypeConfiguration<T> @this,
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

		public static TypeConfiguration<T> EnableReferences<T, TMember>(this TypeConfiguration<T> @this,
		                                                                   Expression<Func<T, TMember>> member)
			=> @this.Member(member)
			        .Identity()
			        .Return(@this);

		public static MemberConfiguration<T, TMember> Identity<T, TMember>(this MemberConfiguration<T, TMember> @this)
			=> @this.Extend<ReferencesExtension>()
			        .Assigned(@this.Type(), @this.Member())
			        .Return(@this);

		public static ICollection<TypeInfo> AllowedReferenceTypes(this IConfigurationElement @this)
			=> @this.Extend<DefaultReferencesExtension>()
			        .Whitelist;

		public static ICollection<TypeInfo> IgnoredReferenceTypes(this IConfigurationElement @this)
			=> @this.Extend<DefaultReferencesExtension>()
			        .Blacklist;
	}
}