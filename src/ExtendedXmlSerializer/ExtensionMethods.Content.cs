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
		/// <summary>
		/// Convenience method for extension authors.  This is used to establish a context to decorate the container's
		/// <see cref="IContents"/> component.
		/// </summary>
		/// <typeparam name="T">The implementation type, of type IContent.</typeparam>
		/// <param name="this">The repository to configure (used within an extension).</param>
		/// <returns>The configured repository.</returns>
		public static ContentsDecorationContext<T> DecorateContentsWith<T>(this IServiceRepository @this)
			where T : IContents
			=> new ContentsDecorationContext<T>(@this);

		/// <summary>
		/// Convenience method for extension authors.  This is used to establish a fluent context which can further be used to
		/// decorate the container's <see cref="IElement"/> component.
		/// </summary>
		/// <typeparam name="T">The implementation type, of type IElement.</typeparam>
		/// <param name="this">The repository to configure.</param>
		/// <returns>The configured repository.</returns>
		public static ElementDecorationContext<T> DecorateElementWith<T>(this IServiceRepository @this)
			where T : IElement
			=> new ElementDecorationContext<T>(@this);

		/* Extension Model: */

		/// <summary>
		/// Assigns a default serialization monitor for a configuration container.  A serialization monitor is a component
		/// that gets notified whenever there is a serialization such as OnSerializing, OnSerialized, as well as
		/// deserialization events such as OnDeserializing, OnDeserialized, etc.
		///
		/// The default serialization monitor is applied for every type that is serialized with the serializer that the
		/// configured container creates.  Use <see cref="ITypeConfiguration{T}.WithMonitor"/> on a type configuration to
		/// apply a monitor to a specific type.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <param name="monitor">The monitor to assign as the default monitor.</param>
		/// <returns>The configured container.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/ExtendedXmlSerializer/issues/264"/>
		public static IConfigurationContainer WithDefaultMonitor(this IConfigurationContainer @this,
		                                                         ISerializationMonitor monitor)
			=> @this.Extend(new SerializationMonitorExtension(monitor));

		/// <summary>
		/// Applies a serialization monitor to a specific type.  A serialization monitor is a component that gets notified
		/// whenever there is a serialization such as OnSerializing, OnSerialized, as well as deserialization events such as
		/// OnDeserializing, OnDeserialized, etc.
		///
		/// Note that calling this method will establish a default monitor if one has not already been assigned.  If you also
		/// want to use a default monitor in addition to type-specific monitors, call the <see cref="WithDefaultMonitor" />
		/// first before calling this method on any types.
		/// </summary>
		/// <typeparam name="T">The type to monitor.</typeparam>
		/// <param name="this">The type configuration to configure.</param>
		/// <param name="monitor">The monitor to apply to the specified type.</param>
		/// <returns>The configured type configuration.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/ExtendedXmlSerializer/issues/264" />
		public static ITypeConfiguration<T> WithMonitor<T>(this ITypeConfiguration<T> @this,
		                                                   ISerializationMonitor<T> monitor)
		{
			@this.Root.With<SerializationMonitorExtension>()
			     .Assign(Support<T>.Key, new SerializationMonitor<T>(monitor));
			return @this;
		}

		/// <summary>
		/// Allows content to be read as parameters for a constructor call to activate an object, rather than the more
		/// traditional route of activating an object and its content read as property assignments.  This is preferred --
		/// required, even -- if your model is comprised of immutable objects.
		///
		/// Note that there are several requirements for a class to be successfully processed:
		/// <list type="number">
		///		<item>only public fields / properties are considered</item>
		///		<item>any public fields (spit) must be readonly</item>
		///		<item>any public properties must have a get but not a set (on the public API, at least)</item>
		///		<item>there must be exactly one interesting constructor, with parameters that are a case-insensitive match for each field/property in some order (i.e. there must be an obvious 1:1 mapping between members and constructor parameter names)</item>
		/// </list>
		/// </summary>
		/// <param name="this">The container to configure.</param>
		/// <returns>The configured container.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/ExtendedXmlSerializer/wiki/04.-Features#immutable-classes-and-content"/>
		public static IConfigurationContainer EnableParameterizedContent(this IConfigurationContainer @this)
			=> @this.Extend(ParameterizedMembersExtension.Default);

		/// <summary>
		/// This is a less strict version of <see cref="EnableParameterizedContent"/>.  Using this version, parameterized
		/// content works the same as <see cref="EnableParameterizedContent"/> but in addition, all properties defined in the
		/// deserialized document are also considered and assigned to the target instance if the property is writable.
		/// </summary>
		/// <param name="this">The container to configure.</param>
		/// <returns>The configured container.</returns>
		public static IConfigurationContainer EnableParameterizedContentWithPropertyAssignments(
			this IConfigurationContainer @this)
			=> @this.Extend(AllParameterizedMembersExtension.Default);

		/// <summary>
		/// Intended for extension authors, and enables a reader context on the deserialization process.  Extension authors
		/// can use <seealso cref="ContentsHistory"/> to retrieve this history of objects being parsed and activated to the
		/// current point of the graph.  This is valuable when parsing object graphs with many internal properties which in
		/// turn have their own set of complex properties.
		/// </summary>
		/// <param name="this">The container to configure.</param>
		/// <returns>The configured container.</returns>
		public static IConfigurationContainer EnableReaderContext(this IConfigurationContainer @this)
			=> @this.Extend(ReaderContextExtension.Default);

		/// <summary>
		/// This is intended to circumvent default behavior which throws an exception for primitive data types when there is
		/// no content provided for their elements.  For example, say you have a boolean element defined as such:
		/// <code>&lt;Boolean /&gt;</code>  Or perhaps the long-form version: <code>&lt;Boolean&gt;&lt;/Boolean&gt;</code>
		///
		/// Either one of these will throw a <seealso cref="FormatException"/>.  Configuring the container with
		/// <seealso cref="EnableImplicitlyDefinedDefaultValues"/> will allow the use of empty values within document
		/// elements.
		/// </summary>
		/// <param name="this">The container to configure.</param>
		/// <returns>The configured container.</returns>
		public static IConfigurationContainer EnableImplicitlyDefinedDefaultValues(this IConfigurationContainer @this)
			=> @this.Alter(ImplicitlyDefinedDefaultValueAlteration.Default);

		/* Emit: */

		public static IConfigurationContainer Emit(this IConfigurationContainer @this, IEmitBehavior behavior)
			=> behavior.Get(@this);

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

		public static ITypeConfiguration<T> EmitWhen<T>(this ITypeConfiguration<T> @this, Func<T, bool> specification)
		{
			@this.Root.With<AllowedInstancesExtension>()
			     .Assign(@this.Get(),
			             new AllowedValueSpecification(new DelegatedSpecification<T>(specification).AdaptForNull()));
			return @this;
		}

		/* Membership: */

		public static IMemberConfiguration<T, TMember> Ignore<T, TMember>(this IMemberConfiguration<T, TMember> @this)
		{
			@this.Root.With<AllowedMembersExtension>()
			     .Blacklist.Add(@this.GetMember());
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
			     .Whitelist.Add(@this.GetMember());
			return @this;
		}

		public static IMemberConfiguration Include(this IMemberConfiguration @this)
		{
			@this.Root.With<AllowedMembersExtension>()
			     .Whitelist.Add(@this.GetMember());
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

		/* Content registration: */

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

		public static IConfigurationContainer Alter(this IConfigurationContainer @this,
		                                            IAlteration<IConverter> alteration)
		{
			@this.Root.With<ConverterAlterationsExtension>()
			     .Alterations.Add(alteration);
			return @this;
		}

		public static IConfigurationContainer Register<T>(this IConfigurationContainer @this, Func<T, string> format,
		                                                  Func<string, T> parse)
			=> @this.Register<T>(new Converter<T>(parse, format));

		public static IConfigurationContainer Register<T>(this IConfigurationContainer @this, IConverter<T> converter)
		{
			var item = converter as Converter<T> ?? Converters<T>.Default.Get(converter);
			return @this.Root.Find<ConvertersExtension>()
			            .Converters
			            .AddOrReplace(item)
			            .Return(@this);
		}

		public static bool Unregister<T>(this IConfigurationContainer @this, IConverter<T> converter)
			=> @this.Root.Find<ConvertersExtension>()
			        .Converters.Removing(converter);

		sealed class Converters<T> : ReferenceCache<IConverter<T>, IConverter<T>>
		{
			public static Converters<T> Default { get; } = new Converters<T>();

			Converters() : base(key => new Converter<T>(key, key.Parse, key.Format)) {}
		}

		#region Obsolete

		[Obsolete(
			"This method is being deprecated and will be removed in a future release. Use Decorate.Element.When instead.")]
		public static IServiceRepository Decorate<T>(this IServiceRepository @this,
		                                             ISpecification<TypeInfo> specification)
			where T : IElement
			=> new ConditionalElementDecoration<T>(specification).Get(@this);

		[Obsolete(
			"This method is being deprecated and will be removed in a future release. Use Decorate.Contents.When instead.")]
		public static IServiceRepository DecorateContent<TSpecification, T>(this IServiceRepository @this)
			where TSpecification : ISpecification<TypeInfo>
			where T : IContents
			=> ConditionalContentDecoration<TSpecification, T>.Default.Get(@this);

		[Obsolete(
			"This method is being deprecated and will be removed in a future release. Use Decorate.Contents.When instead.")]
		public static IServiceRepository DecorateContent<T>(this IServiceRepository @this,
		                                                    ISpecification<TypeInfo> specification) where T : IContents
			=> new ConditionalContentDecoration<T>(specification).Get(@this);

		[Obsolete("This is considered a deprecated feature and will be removed in a future release.")]
		public static IConfigurationContainer OptimizeConverters(this IConfigurationContainer @this)
			=> OptimizeConverters(@this, new Optimizations());

		[Obsolete("This is considered a deprecated feature and will be removed in a future release.")]
		public static IConfigurationContainer OptimizeConverters(this IConfigurationContainer @this,
		                                                         IAlteration<IConverter> optimizations)
			=> @this.Alter(optimizations);

		#endregion
	}
}