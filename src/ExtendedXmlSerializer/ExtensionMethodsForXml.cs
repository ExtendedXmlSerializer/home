using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ExtensionModel.Xml.Classic;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Linq;
using XmlWriter = System.Xml.XmlWriter;

namespace ExtendedXmlSerializer
{
	/// <summary>
	/// Extension methods that assist or enable functionality found within the extension model namespace for xml-specific
	/// behaviors (<see cref="ExtensionModel.Xml"/>).
	/// </summary>
	public static class ExtensionMethodsForXml
	{
		/// <summary>
		/// Configures the provided configuration container to create a serializer that automatically formats its contents
		/// into attributes and elements.  When a serializer encounters a primitive type (or more accurately, a type that has
		/// an <see cref="IConverter"/> registered to handle it), it will automatically serialize its resulting (string) data
		/// as an Xml attribute.  The only exception is when a <see cref="String"/> is encountered, where it will check its
		/// length.  Strings greater than 128 characters will be emitted as inner content.  Otherwise, it will be emitted as
		/// an Xml attribute.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>The configured configuration container.</returns>
		public static IConfigurationContainer UseAutoFormatting(this IConfigurationContainer @this)
			=> @this.Extend(AutoMemberFormatExtension.Default);

		/// <summary>
		/// Configures the provided configuration container to create a serializer that automatically formats its contents
		/// into attributes and elements.  When a serializer encounters a primitive type (or more accurately, a type that has
		/// an <see cref="IConverter"/> registered to handle it), it will automatically serialize its resulting (string) data
		/// as an Xml attribute.  The only exception is when a <see cref="String"/> is encountered, where it will check its
		/// length.  Strings greater than the provided max-length will be emitted as inner content.  Otherwise, it will be
		/// emitted as an Xml attribute.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <param name="maxTextLength">The max length a string can be before it is rendered as inner content.  Any string
		/// shorter than this amount will be rendered as an Xml attribute.</param>
		/// <returns>The configured configuration container.</returns>
		public static IConfigurationContainer UseAutoFormatting(this IConfigurationContainer @this, int maxTextLength)
			=> @this.Extend(new AutoMemberFormatExtension(maxTextLength));

		/// <summary>
		/// Configures the container to create a serializer that consolidates all namespaces so that they emit at the root of
		/// the document, rather than throughout the document when they are first encountered (which can lead to a lot of
		/// unnecessary overhead and larger documents).
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>The configured configuration container.</returns>
		public static IConfigurationContainer UseOptimizedNamespaces(this IConfigurationContainer @this)
			=> @this.Extend(RootInstanceExtension.Default)
			        .Extend(OptimizedNamespaceExtension.Default);

		/// <summary>
		/// Ensures that all text and strings encountered when emitting the document are valid Xml characters, replacing those
		/// that are not with empty strings.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>The configured configuration container.</returns>
		/// <seealso href="https://stackoverflow.com/a/961504/3602057"/>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/ExtendedXmlSerializer/issues/167" />
		public static IConfigurationContainer WithValidCharacters(this IConfigurationContainer @this)
			=> @this.Type<string>().Alter(ValidContentCharacters.Default.Get);

		/// <summary>
		/// Used in dire circumstances.  If you encounter an older .NET object type that cannot be serialized (e.g.
		/// DataTable), and it implements <see cref="ISerializable"/>, call this method to configure the container to create a
		/// serializer that will serialize and deserialize using this interface.
		/// </summary>
		/// <typeparam name="T">The type under configuration.</typeparam>
		/// <param name="this">The type configuration to configure.</param>
		/// <returns>The configured type configuration.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/ExtendedXmlSerializer/issues/268" />
		public static ITypeConfiguration<T> UseClassicSerialization<T>(this ITypeConfiguration<T> @this)
			where T : ISerializable
			=> @this.Register().Serializer().Of<ClassicSerializationAdapter<T>>();

		/// <summary>
		/// Ensures that all text and strings encountered when emitting the specified member are valid Xml characters,
		/// replacing those that are not with empty strings.
		/// </summary>
		/// <typeparam name="T">The member's containing type.</typeparam>
		/// <param name="this">The member configuration to configure.</param>
		/// <returns>The configured member configuration.</returns>
		/// <seealso href="https://stackoverflow.com/a/961504/3602057"/>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/ExtendedXmlSerializer/issues/167" />
		public static IMemberConfiguration<T, string> WithValidCharacters<T>(this IMemberConfiguration<T, string> @this)
			=> @this.Alter(ValidContentCharacters.Default.Get);

		/// <summary>
		/// Configures the specified member to emit as an Xml attribute, rather than as an element.
		/// </summary>
		/// <typeparam name="T">The member's containing type.</typeparam>
		/// <typeparam name="TMember">The value type of the member.</typeparam>
		/// <param name="this">The member configuration to configure.</param>
		/// <returns>The configured member configuration.</returns>
		public static IMemberConfiguration<T, TMember> Attribute<T, TMember>(
			this IMemberConfiguration<T, TMember> @this)
			=> @this.Root.With<MemberFormatExtension>()
			        .Registered.Apply(@this.GetMember())
			        .Return(@this);

		/// <summary>
		/// Configures the specified member to emit as an Xml attribute when the provided condition is met, rather than as an
		/// element.  When the provided condition delegate evaluates as true, the member is emitted as an Xml attribute.
		/// Otherwise, it emits as an Xml element.
		/// </summary>
		/// <typeparam name="T">The member's containing type.</typeparam>
		/// <typeparam name="TMember">The value type of the member.</typeparam>
		/// <param name="this">The member configuration to configure.</param>
		/// <param name="when">The condition used to specify when to render this member as an Xml attribute.</param>
		/// <returns>The configured member configuration.</returns>
		public static IMemberConfiguration<T, TMember> Attribute<T, TMember>(
			this IMemberConfiguration<T, TMember> @this,
			Func<TMember, bool> when)
		{
			@this.Root.With<MemberFormatExtension>()
			     .Specifications[@this.GetMember()] =
				new AttributeSpecification(new DelegatedSpecification<TMember>(when).Adapt());
			return @this.Attribute();
		}

		/// <summary>
		/// Forces a member to emit as an Xml element.  This is only useful if a member was registered as an attribute and for
		/// some reason the member should be further configured to emit as an Xml element instead (effectively delisting it as
		/// an Xml attribute).  Otherwise, emitting as an Xml element is the default behavior and this method should not be used.
		/// </summary>
		/// <typeparam name="T">The member's containing type.</typeparam>
		/// <typeparam name="TMember">The value type of the member.</typeparam>
		/// <param name="this">The member configuration to configure.</param>
		/// <returns>The configured member configuration.</returns>
		public static IMemberConfiguration<T, TMember> Content<T, TMember>(this IMemberConfiguration<T, TMember> @this)
			=> @this.Root.With<MemberFormatExtension>()
			        .Registered.Remove(@this.GetMember())
			        .Return(@this);

		/// <summary>
		/// Forces a member to emit within a CDATA container so it can render its contents verbatim.
		/// </summary>
		/// <typeparam name="T">The member's containing type.</typeparam>
		/// <param name="this">The member configuration to configure.</param>
		/// <returns>The configured member configuration.</returns>
		public static IMemberConfiguration<T, string> Verbatim<T>(this IMemberConfiguration<T, string> @this)
			=> @this.Register().Serializer().Using(VerbatimContentSerializer.Default);

		#region v1

		/// <summary>
		/// This is considered v1 functionality and is not supported, although it is not yet considered deprecated.  Please
		/// make use of the registration methods instead.  The current equivalent for this method call is
		/// IConfigurationContainer.Type{T}.Register().Serializer().Of{TSerializer}.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TSerializer"></typeparam>
		/// <param name="this"></param>
		/// <returns></returns>
		public static ITypeConfiguration<T> CustomSerializer<T, TSerializer>(this IConfigurationContainer @this)
			where TSerializer : IExtendedXmlCustomSerializer<T>
			=> @this.CustomSerializer<T>(typeof(TSerializer));

		/// <summary>
		/// This is considered v1 functionality and is not supported, although it is not yet considered deprecated.  Please
		/// make use of the registration methods instead.  The current equivalent for this method call is
		/// IConfigurationContainer.Type{T}.Register().Serializer().Of(serializerType).
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="this"></param>
		/// <param name="serializerType"></param>
		/// <returns></returns>
		public static ITypeConfiguration<T> CustomSerializer<T>(this IConfigurationContainer @this, Type serializerType)
			=> @this.Type<T>()
			        .CustomSerializer(new ActivatedXmlSerializer(serializerType, Support<T>.Metadata));

		/// <summary>
		/// This is considered v1 functionality and is not supported, although it is not yet considered deprecated.  Please
		/// make use of the registration methods instead.  The current equivalent for this method call is
		/// IConfigurationContainer.Type{T}.Register().Serializer().ByCalling(serializer, deserializer).  Note that the
		/// signatures for the delegates have changed since v1.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="this"></param>
		/// <param name="serializer"></param>
		/// <param name="deserialize"></param>
		/// <returns></returns>
		public static ITypeConfiguration<T> CustomSerializer<T>(this ITypeConfiguration<T> @this,
		                                                        Action<XmlWriter, T> serializer,
		                                                        Func<XElement, T> deserialize)
			=> @this.CustomSerializer(new ExtendedXmlCustomSerializer<T>(deserialize, serializer));

		/// <summary>
		/// This is considered v1 functionality and is not supported, although it is not yet considered deprecated.  Please
		/// make use of the registration methods instead.  The current equivalent for this method call is
		/// IConfigurationContainer.Type{T}.Register().Serializer().Using(serializer).
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="this"></param>
		/// <param name="serializer"></param>
		/// <returns></returns>
		public static ITypeConfiguration<T> CustomSerializer<T>(this ITypeConfiguration<T> @this,
		                                                        IExtendedXmlCustomSerializer<T> serializer)
			=> @this.CustomSerializer(new Adapter<T>(serializer));

		/// <summary>
		/// This is considered v1 functionality and is not supported, although it is not yet considered deprecated.  Please
		/// make use of the registration methods instead.  The current equivalent for this method call is
		/// IConfigurationContainer.Type{T}.Register().Serializer().Using(serializer).
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="this"></param>
		/// <param name="serializer"></param>
		/// <returns></returns>
		public static ITypeConfiguration<T> CustomSerializer<T>(this ITypeConfiguration<T> @this,
		                                                        IExtendedXmlCustomSerializer serializer)
			=> @this.Root.With<CustomSerializationExtension>()
			        .XmlSerializers.Apply(@this.Get(), serializer)
			        .Return(@this);

		/// <summary>
		/// Adds a migration command to the configured type.  A migration allows older persisted XML to migrate to an object
		/// model schema that has changed since the XML was persisted.  The provided command specifies how to manipulate the
		/// element that represents the type so that it can (hopefully 😇) be deserialized without error.
		/// </summary>
		/// <typeparam name="T">The type under configuration.</typeparam>
		/// <param name="this">The type configuration to configure.</param>
		/// <param name="migration">The command that specifies how to migrate an Xml element that represents an older schema.</param>
		/// <returns>The configured type configuration.</returns>
		public static ITypeConfiguration<T> AddMigration<T>(this ITypeConfiguration<T> @this,
		                                                    ICommand<XElement> migration)
			=> @this.AddMigration(migration.Execute);

		/// <summary>
		/// Adds a migration delegate to the configured type.  A migration allows older persisted XML to migrate to an object
		/// model schema that has changed since the XML was persisted.  The provided command specifies how to manipulate the
		/// element that represents the type so that it can (hopefully 😇) be deserialized without error.
		/// </summary>
		/// <typeparam name="T">The type under configuration.</typeparam>
		/// <param name="this">The type configuration to configure.</param>
		/// <param name="migration">The delegate that specifies how to migrate an Xml element that represents an older schema.
		/// </param>
		/// <returns>The configured type configuration.</returns>
		public static ITypeConfiguration<T> AddMigration<T>(this ITypeConfiguration<T> @this,
		                                                    Action<XElement> migration)
			=> @this.AddMigration(migration.Yield());

		/// <summary>
		/// Adds a set of migration delegates to the configured type.  A migration allows older persisted XML to migrate to an
		/// object model schema that has changed since the XML was persisted.  The provided command specifies how to
		/// manipulate the element that represents the type so that it can (hopefully 😇) be deserialized without error.
		/// </summary>
		/// <typeparam name="T">The type under configuration.</typeparam>
		/// <param name="this">The type configuration to configure.</param>
		/// <param name="migrations">The delegates that specify how to migrate an Xml element that represents an older schema.
		/// </param>
		/// <returns>The configured type configuration.</returns>
		public static ITypeConfiguration<T> AddMigration<T>(this ITypeConfiguration<T> @this,
		                                                    IEnumerable<Action<XElement>> migrations)
			=> @this.Root.With<MigrationsExtension>()
			        .Apply(@this.Get(), migrations.Fixed())
			        .Return(@this);

		#endregion
	}
}