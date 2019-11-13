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
	// ReSharper disable once MismatchedFileName
	public static partial class ExtensionMethods
	{
		/* Extension Management 9 */

		public static IExtendedXmlSerializer Create(this IContext @this) => @this.Root.Create();

		public static IRootContext Apply<T>(this IRootContext @this) where T : class, ISerializerExtension
			=> @this.Apply(Support<T>.NewOrSingleton);

		public static IRootContext Apply<T>(this IRootContext @this, Func<T> create)
			where T : class, ISerializerExtension
			=> @this.Contains<T>() ? @this : @this.Apply(create).Return(@this);

		public static T Add<T>(this IRootContext @this) where T : ISerializerExtension
			=> @this.Add(Support<T>.NewOrSingleton);

		public static T Add<T>(this IRootContext @this, Func<T> create) where T : ISerializerExtension
			=> @this.Apply(create()).AsValid<T>();

		public static T With<T>(this IRootContext @this) where T : class, ISerializerExtension
			=> @this.Find<T>() ?? @this.Add<T>();

		public static IRootContext With<T>(this IRootContext @this, Action<T> configure)
			where T : class, ISerializerExtension
			=> configure.Apply(@this.With<T>()).Return(@this);

		public static IRootContext Extend(this IRootContext @this, params ISerializerExtension[] extensions)
		{
			var items = With(@this, extensions).ToList();
			@this.Clear();
			items.ForEach(@this.Add);
			return @this;
		}

		public static ISerializerExtension[] With(this IEnumerable<ISerializerExtension> @this,
		                                          params ISerializerExtension[] extensions)
			=> @this.TypeZip(extensions).ToArray();

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
			        .Root.EnableReferences()
			        .With<ReferencesExtension>()
			        .Apply(@this.Parent.AsValid<ITypeConfigurationContext>().Get(), @this.GetMember())
			        .Return(@this);

		/* Extension Model */

		public static ICollection<TypeInfo> AllowedReferenceTypes(this IConfigurationContainer @this)
			=> @this.Root.With<DefaultReferencesExtension>().Whitelist;

		public static ICollection<TypeInfo> IgnoredReferenceTypes(this IConfigurationContainer @this)
			=> @this.Root.With<DefaultReferencesExtension>().Blacklist;

		public static IConfigurationContainer EnableReferences(this IConfigurationContainer @this)
			=> @this.Root.EnableReferences().Return(@this);

		public static IRootContext EnableReferences(this IRootContext @this)
			=> @this.EnableRootInstances().With<ReferencesExtension>().Return(@this);

		public static IConfigurationContainer EnableDeferredReferences(this IConfigurationContainer @this)
			=> @this.Root.Extend(ReaderContextExtension.Default, DeferredReferencesExtension.Default).Return(@this);

		public static ITypeConfiguration<T> EnableReferences<T, TMember>(this ITypeConfiguration<T> @this,
		                                                                 Expression<Func<T, TMember>> member)
			=> @this.Member(member).Identity().Return(@this);

		#region Obsolete

		[Obsolete(
			"This method is deprecated and will be removed in a future release.  Use IContext.GetTypeConfiguration(Type) instead.")]
		public static ITypeConfiguration GetTypeConfiguration(this IContext @this, TypeInfo type)
			=> @this.GetTypeConfiguration(type.AsType());

		[Obsolete("This will be removed in a future release.  Use IConfigurationContainer.Type<T> instead.")]
		public static ITypeConfiguration<T> ConfigureType<T>(this IConfigurationContainer @this) => @this.Type<T>();

		[Obsolete("This method has been replaced by MemberBy.")]
		public static ITypeConfiguration<T> Member<T>(this ITypeConfiguration<T> @this, MemberInfo member)
		{
			((IInternalTypeConfiguration)@this).Member(member);
			return @this;
		}

		#endregion
	}
}