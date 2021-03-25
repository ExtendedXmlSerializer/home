using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Members;
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
	/// <summary>
	/// Extension methods that assist or enable functionality found within the content model's content namespace (
	/// <see cref="ContentModel.Content"/>).
	/// </summary>
	public static class ExtensionMethodsForContent
	{
		/// <summary>
		/// Convenience method for extension authors.  This is used to establish a context to decorate the container's
		/// <see cref="IContents"/> component.
		/// </summary>
		/// <typeparam name="T">The implementation type, of type <see cref="IContents"/>.</typeparam>
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
		/// Adds support for instances and property types that are explicitly specified as `IEnumerable{T}`.  If an object is
		/// of a type of `IEnumerable` (essentially, a deferred query), it will be evaluated as a <see cref="List{T}"/> and
		/// saved as such as required by either the instance or property in the object graph.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>The configured configuration container.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/home/issues/340" />
		/// <seealso href="https://twitter.com/vcsjones/status/1204432274000879619" />
		public static IConfigurationContainer WithEnumerableSupport(this IConfigurationContainer @this)
			=> @this.Extend(EnumerableSupportExtension.Default);

		/// <summary>
		/// Assigns a default serialization monitor for a configuration container.  A serialization monitor is a component
		/// that gets notified whenever there is a serialization such as OnSerializing, OnSerialized, as well as
		/// deserialization events such as OnDeserializing, OnDeserialized, etc.
		///
		/// The default serialization monitor is applied for every type that is serialized with the serializer that the
		/// configured container creates.  Use <see cref="WithMonitor{T}"/> on a type configuration to
		/// apply a monitor to a specific type.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <param name="monitor">The monitor to assign as the default monitor.</param>
		/// <returns>The configured container.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/home/issues/264"/>
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
		/// <seealso href="https://github.com/ExtendedXmlSerializer/home/issues/264" />
		public static ITypeConfiguration<T> WithMonitor<T>(this ITypeConfiguration<T> @this,
		                                                   ISerializationMonitor<T> monitor)
			=> @this.Root.With<SerializationMonitorExtension>()
			        .Apply(Support<T>.Metadata, new SerializationMonitor<T>(monitor))
			        .Return(@this);

		/// <summary>
		/// Applies an interceptor for type configuration.  An interceptor participates in the serialization pipeline by being
		/// introduced during key serialization and deserialization events.
		/// </summary>
		/// <param name="this">The type to intercept.</param>
		/// <param name="interceptor">The interceptor to apply to a type.</param>
		/// <typeparam name="T">The type argument of the type configuration being configured.</typeparam>
		/// <returns>The configured type configuration.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/home/issues/451" />
		public static ITypeConfiguration<T> WithInterceptor<T>(this ITypeConfiguration<T> @this,
		                                                       ISerializationInterceptor<T> interceptor)
			=> @this.WithInterceptor(new SerializationInterceptor<T>(interceptor));

		/// <summary>
		/// Applies a generalized interceptor for type configuration.  An interceptor participates in the serialization pipeline by being
		/// introduced during key serialization and deserialization events.
		/// </summary>
		/// <param name="this">The type to intercept.</param>
		/// <param name="interceptor">The interceptor to apply to a type.</param>
		/// <typeparam name="T">The type argument of the type configuration being configured.</typeparam>
		/// <returns>The configured type configuration.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/home/issues/451" />
		public static ITypeConfiguration<T> WithInterceptor<T>(this ITypeConfiguration<T> @this,
		                                                       ISerializationInterceptor interceptor)
			=> @this.Root.With<SerializationInterceptionExtension>()
			        .Apply(Support<T>.Metadata, interceptor)
			        .Return(@this);

		/// <summary>
		/// Allows content to be read as parameters for a constructor call to activate an object, rather than the more
		/// traditional route of activating an object and its content read as property assignments.  This is preferred --
		/// required, even -- if your model is comprised of immutable objects.
		///
		/// Note that there are several requirements for a class to be successfully processed:
		///
		///	1. only public fields / properties are considered
		///	1. any public fields (spit) must be readonly
		///	1. any public properties must have a get but not a set (on the public API, at least)
		///	1. there must be exactly one interesting constructor, with parameters that are a case-insensitive match for
		///    each field/property in some order (i.e. there must be an obvious 1:1 mapping between members and constructor
		///	   parameter names)
		/// </summary>
		/// <param name="this">The container to configure.</param>
		/// <returns>The configured container.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/home/wiki/Features#immutable-classes-and-content"/>
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
		/// no content provided for their elements.
		///
		/// For example, say you have a boolean element defined as such: `<Boolean />`  or, perhaps its long-form equivalent `
		/// <Boolean></Boolean>`.
		///
		/// Either one of these by default will throw a <seealso cref="FormatException"/>.  Configuring the container with
		/// <seealso cref="EnableImplicitlyDefinedDefaultValues"/> will allow the use of empty values within document
		/// elements such as the above without throwing an exception.
		/// </summary>
		/// <param name="this">The container to configure.</param>
		/// <returns>The configured container.</returns>
		public static IConfigurationContainer EnableImplicitlyDefinedDefaultValues(this IConfigurationContainer @this)
			=> @this.Alter(ImplicitlyDefinedDefaultValueAlteration.Default);

		/* Emit: */

		/// <summary>
		/// Used to control and determine when content is emitted during serialization.  This is a general-purpose
		/// configuration that works across every type encountered by the serializer. Use the <seealso cref="EmitBehaviors" />
		/// class to utilize one of the built-in (and identified 😁) behaviors, or implement your own
		/// <see cref="IEmitBehavior"/>.
		/// </summary>
		/// <param name="this">The container to configure.</param>
		/// <param name="behavior">The behavior to apply to the container.</param>
		/// <returns>The configured container.</returns>
		public static IConfigurationContainer Emit(this IConfigurationContainer @this, IEmitBehavior behavior)
			=> behavior.Get(@this);

		/// <summary>
		/// Marks the specified type as ignored, meaning it will not emit or read when encountered in a graph.
		/// </summary>
		/// <typeparam name="T">The type under configuration.</typeparam>
		/// <param name="this">The type configuration to configure.</param>
		/// <returns>The configured type configuration.</returns>
		public static ITypeConfiguration<T> Ignore<T>(this ITypeConfiguration<T> @this)
			=> @this.Ignore(@this.Get())
			        .Return(@this);

		/// <summary>
		/// Marks the specified type as ignored, meaning it will not emit or read when encountered in a graph.
		/// </summary>
		/// <param name="this">The container to configure.</param>
		/// <param name="type">The type to mark as ignored.</param>
		/// <returns>The configured container.</returns>
		public static IConfigurationContainer Ignore(this IConfigurationContainer @this, TypeInfo type)
			=> @this.Root.With<AllowedTypesExtension>()
			        .Prohibited.Apply(type)
			        .Return(@this);

		/// <summary>
		/// Marks a type as "allowed" so that it is emitted during serialization and read during deserialization.  Note that
		/// including a type establishes an "allowed-only" policy so that only types that are explicitly included are
		/// considered for processing.
		/// </summary>
		/// <typeparam name="T">The type under configuration.</typeparam>
		/// <param name="this">The type configuration to configure.</param>
		/// <returns>The configured type configuration.</returns>
		public static ITypeConfiguration<T> Include<T>(this ITypeConfiguration<T> @this)
			=> @this.To<ITypeConfiguration>()
			        .Include()
			        .Return(@this);

		/// <summary>
		/// Marks a type as "allowed" so that it is emitted during serialization and read during deserialization.  Note that
		/// including a type establishes an "allowed-only" policy so that only types that are explicitly included are
		/// considered for processing.
		/// </summary>
		/// <param name="this">The type configuration to configure.</param>
		/// <returns>The configured type configuration.</returns>
		public static ITypeConfiguration Include(this ITypeConfiguration @this)
			=> @this.Root.With<AllowedTypesExtension>()
			        .Allowed.Apply(@this.Get())
			        .Return(@this);


		/// <summary>
		/// Configures a member configuration to only emit when its value meets certain criteria.
		/// </summary>
		/// <typeparam name="T">The containing type of the member.</typeparam>
		/// <typeparam name="TMember">The member type.</typeparam>
		/// <param name="this">The member to configure.</param>
		/// <param name="specification">The specification to use to determine whether or not to emit the member, based on value.</param>
		/// <returns>The configured member.</returns>
		public static IMemberConfiguration<T, TMember> EmitWhen<T, TMember>(this IMemberConfiguration<T, TMember> @this,
		                                                                    Func<TMember, bool> specification)
		{
			@this.Root.Find<AllowedMemberValuesExtension>()
			     .Specifications[@this.GetMember()] =
				new AllowedValueSpecification(new DelegatedSpecification<TMember>(specification).AdaptForNull());
			return @this;
		}

		/// <summary>
		/// Configures a member configuration to only emit when a condition of its containing instance is met.  This is useful
		/// for when a data value from another member in another part of the containing instance is needed to determine
		/// whether or not to emit the (currently) configured member.
		/// </summary>
		/// <typeparam name="T">The containing type of the member.</typeparam>
		/// <typeparam name="TMember">The member type.</typeparam>
		/// <param name="this">The member to configure.</param>
		/// <param name="specification"></param>
		/// <returns>The configured member.</returns>
		public static IMemberConfiguration<T, TMember> EmitWhenInstance<T, TMember>(
			this IMemberConfiguration<T, TMember> @this,
			Func<T, bool> specification)
		{
			@this.Root.Find<AllowedMemberValuesExtension>()
			     .Instances[@this.GetMember()] = new DelegatedSpecification<T>(specification).AdaptForNull();
			return @this;
		}

		/// <summary>
		/// Configures a type configuration so that instances of its type only emit when the provided condition is met.
		/// </summary>
		/// <typeparam name="T">The instance type.</typeparam>
		/// <param name="this">The type configuration to configure.</param>
		/// <param name="specification">The specification to determine the condition on when to emit.</param>
		/// <returns>The configured type configuration.</returns>
		public static ITypeConfiguration<T> EmitWhen<T>(this ITypeConfiguration<T> @this, Func<T, bool> specification)
			=> @this.Root.With<AllowedInstancesExtension>()
			        .Apply(@this.Get(),
			               new AllowedValueSpecification(new DelegatedSpecification<T>(specification).AdaptForNull()))
			        .Return(@this);

		/* Membership: */

		/// <summary>
		/// Convenience method to iterate through all explicitly configured types and include all explicitly configured
		/// members.  Only these members will be considered to emit content during serialization as well as reading it
		/// during deserialization.
		/// </summary>
		/// <param name="this">The container to configure.</param>
		/// <returns>The configured container.</returns>
		public static IConfigurationContainer IncludeConfiguredMembers(this IConfigurationContainer @this)
		{
			foreach (var type in @this)
			{
				type.IncludeConfiguredTypeMembers();
			}

			return @this;
		}

		/// <summary>
		/// Convenience method to iterate through all explicitly configured members of a type and mark them as included.  Only
		/// these members will be considered to emit content during serialization as well as reading it during
		/// deserialization.
		/// </summary>
		/// <typeparam name="T">The type under configuration.</typeparam>
		/// <param name="this">The type to configure.</param>
		/// <returns>The configured type.</returns>
		public static ITypeConfiguration<T> IncludeConfiguredMembers<T>(this ITypeConfiguration<T> @this)
			=> @this.IncludeConfiguredTypeMembers().Return(@this);

		static object IncludeConfiguredTypeMembers(this IEnumerable<IMemberConfiguration> @this)
		{
			foreach (var member in @this)
			{
				member.Include();
			}

			return default;
		}

		/// <summary>
		/// Ignores a member so that it is not emitted during serialization, and is not read in during deserialization, even
		/// if the content is specified in the document.  Note that this establishes a "blacklist" policy so that members that
		/// are not ignored get processed.
		/// </summary>
		/// <typeparam name="T">The instance type.</typeparam>
		/// <typeparam name="TMember">The member type.</typeparam>
		/// <param name="this">The member to configure.</param>
		/// <returns>The configured member.</returns>
		public static IMemberConfiguration<T, TMember> Ignore<T, TMember>(this IMemberConfiguration<T, TMember> @this)
			=> @this.Ignore(@this.GetMember())
			        .Return(@this);

		/// <summary>
		/// Ignores a member so that it is not emitted during serialization, and is not read in during deserialization, even
		/// if the content is specified in the document.  Note that this establishes a "blacklist" policy so that members that
		/// are not ignored get processed.
		/// </summary>
		/// <param name="this">The container to configure.</param>
		/// <param name="member">The member to ignore.</param>
		/// <returns>The configured container.</returns>
		public static IConfigurationContainer Ignore(this IConfigurationContainer @this, MemberInfo member)
			=> @this.Root.With<IAllowedMembersExtension>()
			        .Blacklist.Apply(member)
			        .Return(@this);

		/// <summary>
		/// Includes a member so that it is emitted during serialization and read during deserialization.  Note that including
		/// a member establishes a "whitelist" policy so that only members that are explicitly included are considered for processing.
		/// </summary>
		/// <typeparam name="T">The type that contains the member.</typeparam>
		/// <typeparam name="TMember">The type of the member's value.</typeparam>
		/// <param name="this">The member to configure.</param>
		/// <returns>The configured member.</returns>
		public static IMemberConfiguration<T, TMember> Include<T, TMember>(this IMemberConfiguration<T, TMember> @this)
			=> @this.To<IMemberConfiguration>()
			        .Include()
			        .Return(@this);

		/// <summary>
		/// Includes a member so that it is emitted during serialization and read during deserialization.  Note that including
		/// a member establishes a "whitelist" policy so that only members that are explicitly included are considered for
		/// processing.
		/// </summary>
		/// <param name="this">The member to configure.</param>
		/// <returns>The configured member.</returns>
		public static IMemberConfiguration Include(this IMemberConfiguration @this)
			=> @this.Root.With<IAllowedMembersExtension>()
			        .Whitelist.Apply(@this.GetMember())
			        .Return(@this);

		#region Obsolete

		/// <exclude />
		[Obsolete(
			         "This method is being deprecated and will be removed in a future release. Use Decorate.Element.When instead.")]
		public static IServiceRepository Decorate<T>(this IServiceRepository @this,
		                                             ISpecification<TypeInfo> specification)
			where T : IElement
			=> new ConditionalElementDecoration<T>(specification).Get(@this);

		/// <exclude />
		[Obsolete(
			         "This method is being deprecated and will be removed in a future release. Use Decorate.Contents.When instead.")]
		public static IServiceRepository DecorateContent<TSpecification, T>(this IServiceRepository @this)
			where TSpecification : ISpecification<TypeInfo>
			where T : IContents
			=> ConditionalContentDecoration<TSpecification, T>.Default.Get(@this);

		/// <exclude />
		[Obsolete(
			         "This method is being deprecated and will be removed in a future release. Use Decorate.Contents.When instead.")]
		public static IServiceRepository DecorateContent<T>(this IServiceRepository @this,
		                                                    ISpecification<TypeInfo> specification) where T : IContents
			=> new ConditionalContentDecoration<T>(specification).Get(@this);

		/// <exclude />
		[Obsolete("This is considered a deprecated feature and will be removed in a future release.")]
		public static IConfigurationContainer OptimizeConverters(this IConfigurationContainer @this)
			=> OptimizeConverters(@this, new Optimizations());

		/// <exclude />
		[Obsolete("This is considered a deprecated feature and will be removed in a future release.")]
		public static IConfigurationContainer OptimizeConverters(this IConfigurationContainer @this,
		                                                         IAlteration<IConverter> optimizations)
			=> @this.Alter(optimizations);

		/// <exclude />
		[Obsolete(
			         "This method will be removed in a future release.  Use IConfigurationContainer.IncludeConfiguredMembers instead.")]
		public static IConfigurationContainer OnlyConfiguredProperties(this IConfigurationContainer @this)
			=> @this.IncludeConfiguredMembers();

		/// <exclude />
		[Obsolete(
			         "This method will be removed in a future release.  Use ITypeConfiguration<T>.IncludeConfiguredMembers instead.")]
		public static ITypeConfiguration<T> OnlyConfiguredProperties<T>(this ITypeConfiguration<T> @this)
			=> @this.IncludeConfiguredTypeMembers()
			        .Return(@this);

		#endregion
	}
}