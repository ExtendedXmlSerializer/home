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

using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Content.Members;
using System;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	using System.Collections.Generic;
	using System.Reflection;

	public static class Extensions
	{
		public static IConfigurationContainer EnableParameterizedContent(this IConfigurationContainer @this)
			=> @this.Extend(ParameterizedMembersExtension.Default);

		public static IConfigurationContainer EnableReaderContext(this IConfigurationContainer @this)
			=> @this.Extend(ReaderContextExtension.Default);

		public static IConfigurationContainer Emit(this IConfigurationContainer @this, IEmitBehavior behavior) =>
			behavior.Get(@this);

		public static IMemberConfiguration<T, TMember> EmitWhen<T, TMember>(this IMemberConfiguration<T, TMember> @this,
		                                                                Func<TMember, bool> specification)
		{
			@this.Root.Find<AllowedMemberValuesExtension>()
			     .Specifications[((ISource<MemberInfo>)@this).Get()] =
				new AllowedValueSpecification(new DelegatedSpecification<TMember>(specification).AdaptForNull());
			return @this;
		}

		public static IMemberConfiguration<T, TMember> Ignore<T, TMember>(this IMemberConfiguration<T, TMember> @this)
		{
			@this.Root.With<AllowedMembersExtension>()
			     .Blacklist.Add(((ISource<MemberInfo>)@this).Get());
			return @this;
		}

		public static IMemberConfiguration<T, TMember> Include<T, TMember>(this IMemberConfiguration<T, TMember> @this)
		{
			@this.Root.With<AllowedMembersExtension>()
			     .Whitelist.Add(((ISource<MemberInfo>)@this).Get());
			return @this;
		}

		internal static IMemberConfiguration Include(this IMemberConfiguration @this)
		{
			@this.Root.With<AllowedMembersExtension>()
				.Whitelist.Add(((ISource<MemberInfo>)@this).Get());
			return @this;
		}

		public static IConfigurationContainer OnlyConfiguredProperties(this IConfigurationContainer @this)
		{
			foreach (var type in @this)
			{
				type.OnlyConfiguredProperties();
			}
			return @this;
		}

		public static ITypeConfiguration<T> OnlyConfiguredProperties<T>(this ITypeConfiguration<T> @this)
		{
			foreach (var member in (IEnumerable<IMemberConfiguration>)@this)
			{
				member.Include();
			}
			return @this;
		}

		public static IConfigurationContainer Alter(this IConfigurationContainer @this, IAlteration<IConverter> alteration)
		{
			@this.Root.With<ConverterAlterationsExtension>()
			     .Alterations.Add(alteration);
			return @this;
		}

		public static IConfigurationContainer EnableImplicitlyDefinedDefaultValues(this IConfigurationContainer @this)
			=> @this.Alter(ImplicitlyDefinedDefaultValueAlteration.Default);

		public static IConfigurationContainer OptimizeConverters(this IConfigurationContainer @this)
			=> OptimizeConverters(@this, new Optimizations());

		public static IConfigurationContainer OptimizeConverters(this IConfigurationContainer @this,
		                                                         IAlteration<IConverter> optimizations)
			=> @this.Alter(optimizations);

		public static IConfigurationContainer Register<T>(this IConfigurationContainer @this, IConverter<T> converter)
		{
			var item = converter as Converter<T> ?? Converters<T>.Default.Get(converter);
			@this.Root.Find<ConvertersExtension>()
			     .Converters
			     .AddOrReplace(item);
			return @this;
		}

		public static bool Unregister<T>(this IConfigurationContainer @this, IConverter<T> converter)
			=> @this.Root.Find<ConvertersExtension>()
			        .Converters.Removing(converter);

		sealed class Converters<T> : ReferenceCache<IConverter<T>, IConverter<T>>
		{
			public static Converters<T> Default { get; } = new Converters<T>();
			Converters() : base(key => new Converter<T>(key, key.Parse, key.Format)) {}
		}
	}
}