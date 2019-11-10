using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Content.Members;
using ExtendedXmlSerializer.ExtensionModel.Instances;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer
{
	// ReSharper disable once MismatchedFileName
	public static partial class ExtensionMethods
	{
		public static ITypeConfiguration<T> RegisterContentComposition<T>(this ITypeConfiguration<T> @this,
		                                                                  Func<ISerializer<T>, ISerializer<T>> compose)
			=> @this.RegisterContentComposition(new SerializerComposer<T>(compose).Get);

		public static ITypeConfiguration<T> RegisterContentComposition<T>(this ITypeConfiguration<T> @this,
		                                                                  Func<ISerializer, ISerializer> compose)
			=> @this.RegisterContentComposition(new SerializerComposer(compose));

		public static ITypeConfiguration<T> RegisterContentComposition<T>(this ITypeConfiguration<T> @this,
		                                                                  ISerializerComposer composer)
		{
			@this.Root.With<RegisteredCompositionExtension>()
			     .Assign(Support<T>.Key, composer);
			return @this;
		}

		public static IConfigurationContainer WithDefaultMonitor(this IConfigurationContainer @this,
		                                                         ISerializationMonitor monitor)
			=> @this.Extend(new SerializationMonitorExtension(monitor));

		public static ITypeConfiguration<T> WithMonitor<T>(this ITypeConfiguration<T> @this,
		                                                   ISerializationMonitor<T> monitor)
		{
			@this.Root.With<SerializationMonitorExtension>()
			     .Assign(Support<T>.Key, new SerializationMonitor<T>(monitor));
			return @this;
		}

		public static IServiceRepository Decorate<T>(this IServiceRepository @this,
		                                             ISpecification<TypeInfo> specification)
			where T : IElement
			=> new ConditionalElementDecoration<T>(specification).Get(@this);

		public static IServiceRepository DecorateContent<TSpecification, T>(this IServiceRepository @this)
			where TSpecification : ISpecification<TypeInfo>
			where T : IContents
			=> ConditionalContentDecoration<TSpecification, T>.Default.Get(@this);

		public static IServiceRepository DecorateContent<T>(this IServiceRepository @this,
		                                                    ISpecification<TypeInfo> specification) where T : IContents
			=> new ConditionalContentDecoration<T>(specification).Get(@this);

		public static IConfigurationContainer EnableParameterizedContent(this IConfigurationContainer @this)
			=> @this.Extend(ParameterizedMembersExtension.Default);

		public static IConfigurationContainer EnableParameterizedContentWithPropertyAssignments(
			this IConfigurationContainer @this)
			=> @this.Extend(AllParameterizedMembersExtension.Default);

		public static IConfigurationContainer EnableReaderContext(this IConfigurationContainer @this)
			=> @this.Extend(ReaderContextExtension.Default);

		public static IConfigurationContainer Emit(this IConfigurationContainer @this, IEmitBehavior behavior) =>
			behavior.Get(@this);

		public static IMemberConfiguration<T, TMember> EmitWhen<T, TMember>(this IMemberConfiguration<T, TMember> @this,
		                                                                    Func<TMember, bool> specification)
		{
			@this.Root.Find<AllowedMemberValuesExtension>()
			     .Specifications[@this.GetMember()] =
				new AllowedValueSpecification(new DelegatedSpecification<TMember>(specification).AdaptForNull());
			return @this;
		}

		public static IMemberConfiguration<T, TMember> EmitWhenInstance<T, TMember>(
			this IMemberConfiguration<T, TMember> @this,
			Func<T, bool> specification)
		{
			@this.Root.Find<AllowedMemberValuesExtension>()
			     .Instances[@this.GetMember()] = new DelegatedSpecification<T>(specification).AdaptForNull();
			return @this;
		}

		public static ITypeConfiguration<T> EmitWhen<T>(this ITypeConfiguration<T> @this,
		                                                Func<T, bool> specification)
		{
			@this.Root.With<AllowedInstancesExtension>()
			     .Assign(@this.Get(),
			             new AllowedValueSpecification(new DelegatedSpecification<T>(specification).AdaptForNull()));
			return @this;
		}

		public static IMemberConfiguration<T, TMember> Ignore<T, TMember>(this IMemberConfiguration<T, TMember> @this)
		{
			@this.Root.With<AllowedMembersExtension>()
			     .Blacklist.Add(((ISource<MemberInfo>)@this).Get());
			return @this;
		}

		public static IConfigurationContainer Ignore(this IConfigurationContainer @this, MemberInfo member)
		{
			@this.Root.With<AllowedMembersExtension>()
			     .Blacklist.Add(member);
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

		public static IConfigurationContainer Alter(this IConfigurationContainer @this,
		                                            IAlteration<IConverter> alteration)
		{
			@this.Root.With<ConverterAlterationsExtension>()
			     .Alterations.Add(alteration);
			return @this;
		}

		public static IConfigurationContainer EnableImplicitlyDefinedDefaultValues(this IConfigurationContainer @this)
			=> Alter(@this, ImplicitlyDefinedDefaultValueAlteration.Default);

		public static IConfigurationContainer OptimizeConverters(this IConfigurationContainer @this)
			=> OptimizeConverters(@this, new Optimizations());

		public static IConfigurationContainer OptimizeConverters(this IConfigurationContainer @this,
		                                                         IAlteration<IConverter> optimizations)
			=> @this.Alter(optimizations);

		public static IConfigurationContainer Register<T>(this IConfigurationContainer @this, Func<T, string> format,
		                                                  Func<string, T> parse)
			=> @this.Register<T>(new Converter<T>(parse, format));

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
