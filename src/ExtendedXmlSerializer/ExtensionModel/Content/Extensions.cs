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

using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Content.Members;
using ExtendedXmlSerializer.ExtensionModel.Services;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	public static class ExtensionMethods
	{
		public static IServiceRepository Decorate<T>(this IServiceRepository @this, ISpecification<TypeInfo> specification)
			where T : IElements
			=> new ConditionalElementDecoration<T>(specification).Get(@this);


		public static IServiceRepository Decorate(this IServiceRepository @this, Type from, Type to, Type other)
			=> @this.Register(other)
			        .Decorate(from, to);

		public static IServiceRepository Decorate<T>(this IServiceRepository @this) where T : IContents<object>
			=> @this.DecorateDefinition<IContents<object>, T>();

		public static IServiceRepository DecorateContent<TSpecification, T>(this IServiceRepository @this)
			where TSpecification : ISpecification<TypeInfo>
			where T : IContents
			=> ConditionalContentDecoration<TSpecification, T>.Default.Get(@this);

		public static IServiceRepository DecorateContent<T>(this IServiceRepository @this,
		                                                    ISpecification<TypeInfo> specification) where T : IContents
			=> new ConditionalContentDecoration<T>(specification).Get(@this);

		public static IConfigurationElement EnableParameterizedContent(this IConfigurationElement @this)
			=> @this.Extended<ParameterizedMembersExtension>();

		public static IConfigurationElement EnableReaderContext(this IConfigurationElement @this)
			=> @this.Extended<ReaderContextExtension>();

		public static IConfigurationElement Emit(this IConfigurationElement @this, IEmitBehavior behavior) =>
			behavior.Get(@this);

		public static IType<T> EmitWhen<T>(this IType<T> @this,
		                                                  Func<T, bool> specification)
			=> @this.Extend<AllowedInstancesExtension>()
			        .Assigned(@this.Type(),
			                  new AllowedValueSpecification(new DelegatedSpecification<T>(specification).AdaptForNull()))
			        .Return(@this);

		public static MemberConfiguration<T, TMember> EmitWhen<T, TMember>(this MemberConfiguration<T, TMember> @this,
		                                                                    Func<TMember, bool> specification)
		{
			@this.Extend<AllowedMemberValuesExtension>()
			     .Specifications[@this.Member()] =
				new AllowedValueSpecification(new DelegatedSpecification<TMember>(specification).AdaptForNull());
			return @this;
		}

		public static MemberConfiguration<T, TMember> Ignore<T, TMember>(this MemberConfiguration<T, TMember> @this)
			=> @this.Extend<MemberPolicyExtension>()
			        .Blacklist.Adding(@this.Member())
			        .Return(@this);

		public static T Include<T>(this T @this) where T : class, IMemberConfiguration
			=> @this.Extend<MemberPolicyExtension>()
			        .Whitelist.Adding(@this.Member())
			        .Return(@this);

		public static T OnlyConfiguredProperties<T>(this T @this) where T : class, IConfigurationElement
		{
			foreach (var type in @this.Service<IConfiguredTypes>())
			{
				type.OnlyConfiguredMembers();
			}

			return @this;
		}

		public static T OnlyConfiguredMembers<T>(this T @this) where T : class, ITypeConfiguration
		{
			foreach (var member in @this.Members())
			{
				member.Include();
			}

			return @this;
		}

		public static IConfigurationElement Alter(this IConfigurationElement @this, IAlteration<IConverter> alteration)
		{
			@this.Extend<ConverterAlterationsExtension>()
			     .Alterations.Add(alteration);
			return @this;
		}

		public static IConfigurationElement EnableImplicitlyDefinedDefaultValues(this IConfigurationElement @this)
			=> @this.Alter(ImplicitlyDefinedDefaultValueAlteration.Default);

		public static IConfigurationElement OptimizeConverters(this IConfigurationElement @this)
			=> OptimizeConverters(@this, new Optimizations());

		public static IConfigurationElement OptimizeConverters(this IConfigurationElement @this,
		                                                         IAlteration<IConverter> optimizations)
			=> @this.Alter(optimizations);

		public static IConfigurationElement Register<T>(this IConfigurationElement @this, IConverter<T> converter)
			=> @this.Unregister(converter).Return(@this).Extend<ConvertibleContentsExtension>()
			        .Converters
			        .Adding(converter as Converter<T> ?? Converters<T>.Default.Get(converter))
			        .Return(@this);

		public static bool Unregister<T>(this IConfigurationElement @this, IConverter<T> converter)
			=> @this.Extend<ConvertibleContentsExtension>()
			        .Converters.Remove(converter as Converter<T> ?? Converters<T>.Default.Get(converter));

		sealed class Converters<T> : ReferenceCache<IConverter<T>, Converter<T>>
		{
			public static Converters<T> Default { get; } = new Converters<T>();
			Converters() : base(key => new Converter<T>(key, key.Parse, key.Format)) {}
		}
	}
}