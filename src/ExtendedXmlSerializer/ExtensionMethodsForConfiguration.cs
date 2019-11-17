using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.References;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExtendedXmlSerializer
{
	/// <summary>
	/// Extension methods that assist or enable functionality found within the configuration namespace (
	/// <see cref="Configuration"/>).
	/// </summary>
	// ReSharper disable once ClassTooBig
	public static class ExtensionMethodsForConfiguration
	{
		/// <summary>
		/// The main event.  Used to create a new serializer from the configured context (usually a <see cref="IConfigurationContainer"/>).
		/// </summary>
		/// <param name="this">The configured context that creates the serializer.</param>
		/// <returns>The configured serializer.</returns>
		public static IExtendedXmlSerializer Create(this IContext @this) => @this.Root.Create();

		/// <summary>
		/// Used to apply a new serializer extension of the provided type.  If an extension already exists in the provided
		/// context, it is returned.  Otherwise, it will attempt to locate a singleton on the provided type, and if that isn't
		/// found, activate it by calling its public constructor.
		/// </summary>
		/// <typeparam name="T">The serializer extension type to apply.</typeparam>
		/// <param name="this">The configuration context (usually a configuration container) to locate the provided serializer
		/// extension type.</param>
		/// <returns>The configured context with the requested extension applied to it.</returns>
		public static IRootContext Apply<T>(this IRootContext @this) where T : class, ISerializerExtension
			=> @this.Apply(Support<T>.NewOrSingleton);

		/// <summary>
		/// Used to apply a new serializer extension of the provided type.  If an extension already exists in the provided
		/// context, it is returned.  Otherwise, it will use the provided factory to create the serializer and register it
		/// with the provided context.
		/// </summary>
		/// <typeparam name="T">The serializer extension type to apply.</typeparam>
		/// <param name="this">The configuration context (usually a configuration container) to locate the provided serializer
		/// extension type.</param>
		/// <param name="create">The factory used to create the extension of the requested type, if an instance of its type
		/// does not already exist.</param>
		/// <returns>The configured context with the requested extension applied to it.</returns>
		public static IRootContext Apply<T>(this IRootContext @this, Func<T> create)
			where T : class, ISerializerExtension
			=> @this.Contains<T>() ? @this : @this.Add(create).Return(@this);

		/// <summary>
		/// Adds an extension of the provided type to the provided context.  This will be done by attempting to locate a
		/// singleton on the provided type, and if that isn't found, activate it by calling its public constructor.
		/// </summary>
		/// <typeparam name="T">The serializer extension type to locate and add.</typeparam>
		/// <param name="this">The configuration context (usually a configuration container) with which to add the created
		/// serializer extension.</param>
		/// <returns>The created and added extension.</returns>
		public static T Add<T>(this IRootContext @this) where T : ISerializerExtension
			=> @this.Add(Support<T>.NewOrSingleton);

		/// <summary>
		/// Adds an extension to the provided context by invoking the provided factory method and adding it to the context.
		/// </summary>
		/// <typeparam name="T">The serializer extension type to create and add.</typeparam>
		/// <param name="this">The configuration context (usually a configuration container) with which to add the created
		/// serializer extension.</param>
		/// <param name="create">The factory used to create the extension of the requested type.</param>
		/// <returns>The created and added extension.</returns>
		public static T Add<T>(this IRootContext @this, Func<T> create) where T : ISerializerExtension
			=> @this.Apply(create()).AsValid<T>();

		/// <summary>
		/// Finds or creates/add the requested serializer extension type.  If an extension of the requested type already
		/// exists, it is returned.  Otherwise, a new one is created by searching first for a singleton on the requested type,
		/// and creating a new instance by way of public constructor if not.
		/// </summary>
		/// <typeparam name="T">The requested serializer extension type.</typeparam>
		/// <param name="this">The root context to search for a serializer extension of provided type.</param>
		/// <returns>The located or created serializer extension.</returns>
		public static T With<T>(this IRootContext @this) where T : class, ISerializerExtension
			=> @this.Find<T>() ?? @this.Add<T>();

		/// <summary>
		/// Finds or creates/add the requested serializer extension type, and then configures it with the provided action once
		/// it does.
		/// </summary>
		/// <typeparam name="T">The requested serializer extension type.</typeparam>
		/// <param name="this">The root context to search for a serializer extension of provided type.</param>
		/// <param name="configure">The configuration action to invoke once the serializer extension has been located.</param>
		/// <returns>The configured context (usually a configuration container).</returns>
		public static IRootContext With<T>(this IRootContext @this, Action<T> configure)
			where T : class, ISerializerExtension
			=> configure.Apply(@this.With<T>()).Return(@this);

		/// <summary>
		/// Used to extend a root context (usually a configuration container).  This passes in a collection of extensions to
		/// add to the context's collection of serializer extensions.
		/// </summary>
		/// <param name="this">The root context that contains the target collection of serializer extensions.</param>
		/// <param name="extensions">The array of extensions to add.</param>
		/// <returns>The configured context (usually a configuration container).</returns>
		public static IRootContext Extend(this IRootContext @this, params ISerializerExtension[] extensions)
		{
			var items = @this.TypeZip(extensions).ToList();
			@this.Clear();
			items.ForEach(@this.Add);
			return @this;
		}

		/* Container */

		/// <summary>
		/// Configures the container with a configuration profile.  A configuration profile is a profile of configurations
		/// that can be applied to a configuration container.  It is a way of preserving commonly-used configurations and
		/// applying them quickly to a configuration container.
		/// </summary>
		/// <typeparam name="T">The type of the configuration profile.</typeparam>
		/// <param name="this">The container to configure.</param>
		/// <returns>The configured container with the configuration profile applied to it.</returns>
		public static IConfigurationContainer Configured<T>(this IConfigurationContainer @this)
			where T : class, IConfigurationProfile
			=> Support<T>.NewOrSingleton().Get(@this);

		/// <summary>
		/// Gets or creates a type configuration from the container.  Type configurations are configurations that deal
		/// specifically with a particular type and allow you to query its member configurations for further configuration of
		/// the type's members.
		/// </summary>
		/// <typeparam name="T">The requested type.</typeparam>
		/// <param name="this">The container from which to request the type configuration.</param>
		/// <returns>The type configuration.</returns>
		public static ITypeConfiguration<T> Type<T>(this IConfigurationContainer @this)
			=> @this.GetTypeConfiguration(Support<T>.Key).AsValid<TypeConfiguration<T>>();

		/// <summary>
		/// Gets or creates a type configuration from the container, and then configures it with the provided action.  Type
		/// configurations are configurations that deal specifically with a particular type and allow you to query its member
		/// configurations for further configuration of the type's members.
		/// </summary>
		/// <typeparam name="T">The requested type.</typeparam>
		/// <param name="this">The container from which to request the type configuration.</param>
		/// <param name="configure">The configuration to perform on the type configuration once it has been retrieved.</param>
		/// <returns>The configured type configuration.</returns>
		public static IConfigurationContainer Type<T>(this IConfigurationContainer @this,
		                                              Action<ITypeConfiguration<T>> configure)
			=> configure.Apply(@this.Type<T>()).Return(@this);

		/// <summary>
		/// Gets or creates a type configuration from the container.  Type configurations are configurations that deal
		/// specifically with a particular type and allow you to query its member configurations for further configuration of
		/// the type's members.
		/// </summary>
		/// <param name="this">The context (usually a configuration containers) from which to request the type configuration.</param>
		/// <param name="type">The type to retrieve.</param>
		/// <returns>The type configuration.</returns>
		public static ITypeConfiguration GetTypeConfiguration(this IContext @this, Type type)
			=> @this.Root.Types.Get(type);

		/* Type */

		/// <summary>
		/// Applies a name for the type.  This provided name will be used to emit the type when needed during serialization,
		/// and again when needed during the reading of a deserialization.
		/// </summary>
		/// <typeparam name="T">The type under configuration.</typeparam>
		/// <param name="this">The type configuration to configure.</param>
		/// <param name="name">The name to apply to the type.</param>
		/// <returns>The configured type configuration.</returns>
		public static ITypeConfiguration<T> Name<T>(this ITypeConfiguration<T> @this, string name)
			=> @this.AsInternal().Name(name).Return(@this);

		/// <summary>
		/// Get (or create) the member configuration from the type that resolves with the provided expression.
		/// </summary>
		/// <typeparam name="T">The type under configuration.</typeparam>
		/// <typeparam name="TMember">The type of the member's value.</typeparam>
		/// <param name="this">The type configuration to configure.</param>
		/// <param name="member">The expression to that resolves to a member of the type under configuration.</param>
		/// <returns>The requested member configuration.</returns>
		public static IMemberConfiguration<T, TMember> Member<T, TMember>(this ITypeConfiguration<T> @this,
		                                                                  Expression<Func<T, TMember>> member)
			=> @this.AsInternal()
			        .Member(member.GetMemberInfo())
			        .AsValid<MemberConfiguration<T, TMember>>();

		/// <summary>
		/// Get (or create) the member configuration from the type that resolves with the provided expression, and then
		/// configures it with the provided action.
		/// </summary>
		/// <typeparam name="T">The type under configuration.</typeparam>
		/// <typeparam name="TMember">The type of the member's value.</typeparam>
		/// <param name="this">The type configuration to configure.</param>
		/// <param name="member">The expression to that resolves to a member of the type under configuration.</param>
		/// <param name="configure">The configuration to perform on the member configuration once retrieved.</param>
		/// <returns>The configured type configuration.</returns>
		public static ITypeConfiguration<T> Member<T, TMember>(this ITypeConfiguration<T> @this,
		                                                       Expression<Func<T, TMember>> member,
		                                                       Action<IMemberConfiguration<T, TMember>> configure)
			=> configure.Apply(@this.Member(member)).Return(@this);

		internal static IMemberConfiguration Member(this ITypeConfiguration @this, string member)
		{
			var metadata = @this.Get()
			                    .GetMember(member)
			                    .SingleOrDefault();
			var result = metadata != null ? @this.AsInternal().Member(metadata) : null;
			return result;
		}

		/// <summary>
		/// Get (or create) the member configuration from the type that resolves with the provided member metadata.
		/// </summary>
		/// <typeparam name="T">The type under configuration.</typeparam>
		/// <param name="this">The type configuration to configure.</param>
		/// <param name="member">The metadata used to query the type.</param>
		/// <returns>The located member configuration.</returns>
		public static IMemberConfiguration MemberBy<T>(this ITypeConfiguration<T> @this, MemberInfo member)
			=> @this.AsInternal().Member(member);

		/// <summary>
		/// Get (or create) the member configuration from the type that resolves with the provided strongly-typed member
		/// metadata.  Strongly-typed member metadata can be created via the use of the <see cref="As{T}"/> method.
		/// </summary>
		/// <typeparam name="T">The type under configuration.</typeparam>
		/// <typeparam name="TMember">The value type of the member.</typeparam>
		/// <param name="this">The type configuration to configure.</param>
		/// <param name="member">The strongly-typed metadata used to query the type.</param>
		/// <returns>The located member configuration.</returns>
		public static IMemberConfiguration<T, TMember> MemberBy<T, TMember>(this ITypeConfiguration<T> @this,
		                                                                    MemberInfo<TMember> member)
			=> @this.MemberBy(member.Get()).AsValid<MemberConfiguration<T, TMember>>();

		/* Member */

		/// <summary>
		/// Applies a name for the member.  This will result in emitting an element name for the member with the provided
		/// value during serialization, as well as reading the name from XML elements during deserialization.
		/// </summary>
		/// <typeparam name="T">The containing type of the member.</typeparam>
		/// <typeparam name="TMember">The type of the member's value.</typeparam>
		/// <param name="this">The member configuration to configure.</param>
		/// <param name="name">The name to assign for the member under configuration.</param>
		/// <returns>The configured member configuration.</returns>
		public static IMemberConfiguration<T, TMember> Name<T, TMember>(this IMemberConfiguration<T, TMember> @this,
		                                                                string name)
			=> @this.AsInternal().Name(name).Return(@this);

		/// <summary>
		/// Sets the order for the given member.  This is used in ordering all elements when they are emitted during serialization.
		/// </summary>
		/// <typeparam name="T">The containing type of the member.</typeparam>
		/// <typeparam name="TMember">The member's value type.</typeparam>
		/// <param name="this">The member configuration which to order.</param>
		/// <param name="order">The desired order value for the member.</param>
		/// <returns>The configured member configuration.</returns>
		public static IMemberConfiguration<T, TMember> Order<T, TMember>(this IMemberConfiguration<T, TMember> @this,
		                                                                 int order)
			=> @this.AsInternal().Order(order).Return(@this);

		/// <summary>
		/// Convenience method to create a strongly-typed MemberInfo object that can be used to query from a configuration
		/// container via the <see cref="MemberBy{T}"/> method call.
		/// </summary>
		/// <typeparam name="T">The value type of the member.</typeparam>
		/// <param name="this">The member to use as the source.</param>
		/// <returns>A strongly-typed MemberInfo instance.</returns>
		public static MemberInfo<T> As<T>(this MemberInfo @this) => new MemberInfo<T>(@this);

		/// <summary>
		/// Flags the provided member configuration as the identity member for the container's references.  Once an identity
		/// member is established, it is used to emit its unique value and to later read it during deserialization.  The
		/// unique value is used to keep track of references in a different application and/or domain context from when the
		/// original serialization occurred.
		/// </summary>
		/// <typeparam name="T">The containing type of the member.</typeparam>
		/// <typeparam name="TMember">The member's value type.</typeparam>
		/// <param name="this">The member configuration to configure.</param>
		/// <returns>The configured member configuration.</returns>
		public static IMemberConfiguration<T, TMember> Identity<T, TMember>(this IMemberConfiguration<T, TMember> @this)
			=> @this.Attribute()
			        .Root.EnableRootReferences()
			        .With<ReferencesExtension>()
			        .Apply(@this.Parent.AsValid<ITypeConfigurationContext>().Get(), @this.GetMember())
			        .Return(@this);

		/* Extension Model */

		/// <summary>
		/// Retrieves the current "whitelist" of allowed types on a configuration container.  If specified and populated,
		/// these are the only types that can have
		/// <see cref="EnableReferences(IConfigurationContainer)"/> called on them.  Note
		/// that if both whitelist and blacklists are populated, the whitelist takes precedence.
		/// </summary>
		/// <param name="this">The configuration container to query.</param>
		/// <returns>The current allowed types that can be reference-enabled.</returns>
		public static ICollection<TypeInfo> AllowedReferenceTypes(this IConfigurationContainer @this)
			=> @this.Root.With<DefaultReferencesExtension>().Whitelist;

		/// <summary>
		/// Retrieves the current "blacklist" of ignored types on a configuration container.  By default this is the see
		/// cref="String"/> type. If specified and populated, these are the only types that cannot have
		/// <see cref="EnableReferences(IConfigurationContainer)"/> called on them.  Note that if both whitelist and
		/// blacklists are populated, the whitelist takes precedence.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>A collection of types that cannot be reference-enabled.</returns>
		public static ICollection<TypeInfo> IgnoredReferenceTypes(this IConfigurationContainer @this)
			=> @this.Root.With<DefaultReferencesExtension>().Blacklist;

		/// <summary>
		/// Enables references on a configuration container, which will create a serializer that supports circular references.
		/// When the first reference is encountered, it will be emitted. Further occurrences of the same reference will emit
		/// with a special attribute along with its unique value.  This allows circular references to be serialized and
		/// subsequently deserialized appropriately and properly.  Note that, by default, if this method is not invoked on a
		/// configuration container, and a serializer that it creates attempts to serialize an object with circular
		/// references, an exception is thrown.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>The configured configuration container.</returns>
		public static IConfigurationContainer EnableReferences(this IConfigurationContainer @this)
			=> @this.Root.EnableRootReferences().Return(@this);


		/// <summary>
		/// Enables references on a configuration container, which will create a serializer that supports circular references.
		/// Additionally, this call will register a particular type as allowing references, and establish the member that
		/// evaluates with the provided expression as the identity member.  Doing so will allow the serializer to keep track
		/// of references based on the unique values found with the identity member.
		/// </summary>
		/// <typeparam name="T">The type under configuration.</typeparam>
		/// <typeparam name="TMember">The resulting identity member value type.</typeparam>
		/// <param name="this">The type configuration under configuration.</param>
		/// <param name="member">The member expression that is intended to resolve as the identity member for the type
		/// configuration.</param>
		/// <returns>The configured type configuration.</returns>
		public static ITypeConfiguration<T> EnableReferences<T, TMember>(this ITypeConfiguration<T> @this,
		                                                                 Expression<Func<T, TMember>> member)
			=> @this.Member(member).Identity().Return(@this);

		/// <summary>
		/// This is an alternative version of enabling references on a configuration container and the subsequent serializers
		/// that it creates.  It works much like
		/// <see cref="EnableReferences(IConfigurationContainer)"/>, except that it will
		/// defer emitting the identity references until the last one is encountered.  By contrast,
		/// <see cref="EnableReferences(IConfigurationContainer)"/> emits the identity
		/// references when it first encounters them, and then a reference back to the identity reference with each subsequent
		/// encounter.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>The configured configuration container.</returns>
		public static IConfigurationContainer EnableDeferredReferences(this IConfigurationContainer @this)
			=> @this.Root.Extend(ReaderContextExtension.Default, DeferredReferencesExtension.Default).Return(@this);

		static IRootContext EnableRootReferences(this IRootContext @this)
			=> @this.EnableRootInstances().With<ReferencesExtension>().Return(@this);

		#region Obsolete

		/// <exclude />
		[Obsolete("This is considered deprecated and will be removed in a future release.  Use IRootContext.Extend instead.")]
		public static ISerializerExtension[] With(this IEnumerable<ISerializerExtension> @this,
		                                          params ISerializerExtension[] extensions)
			=> @this.TypeZip(extensions).ToArray();

		/// <exclude />
		[Obsolete("This is considered deprecated and will be removed in a future release.")]
		public static IRootContext EnableReferences(this IRootContext @this)
			=> @this.EnableRootInstances().With<ReferencesExtension>().Return(@this);

		/// <exclude />
		[Obsolete(
			"This method is deprecated and will be removed in a future release.  Use IContext.GetTypeConfiguration(Type) instead.")]
		public static ITypeConfiguration GetTypeConfiguration(this IContext @this, TypeInfo type)
			=> @this.GetTypeConfiguration(type.AsType());

		/// <exclude />
		[Obsolete("This will be removed in a future release.  Use IConfigurationContainer.Type<T> instead.")]
		public static ITypeConfiguration<T> ConfigureType<T>(this IConfigurationContainer @this) => @this.Type<T>();

		/// <exclude />
		[Obsolete("This method has been replaced by MemberBy.")]
		public static ITypeConfiguration<T> Member<T>(this ITypeConfiguration<T> @this, MemberInfo member)
			=> @this.AsInternal().Member(member).Return(@this);

		#endregion
	}
}