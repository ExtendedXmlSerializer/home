using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Runtime.Serialization;

namespace ExtendedXmlSerializer
{
	/// <summary>
	/// A set of extension methods to assist in registration operations for the container, its types, and/or its type's members.
	/// Registrations can be serializers or converters to change the default behavior on how these components emit or read
	/// in their content.
	/// </summary>
	public static class ExtensionMethodsForRegistration
	{
		/// <summary>
		/// Registers the provided surrogate provider with the provided configuration container container.  The surrogate
		/// provider will be used to query for selecting a serializer if it contains one that satisfies the requested type.
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <param name="provider">The provider to register.</param>
		/// <returns>The configured configuration container.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/ExtendedXmlSerializer/issues/161" />
		public static IConfigurationContainer Register(this IConfigurationContainer @this,
		                                               ISerializationSurrogateProvider provider)
			=> @this.Extend(new SurrogatesExtension(provider));

		/// <summary>
		/// Establishes a registration context for the specified type configuration.  From there, you can perform registration
		/// operations on serializers and converters for the type.
		/// </summary>
		/// <typeparam name="T">The type under configuration.</typeparam>
		/// <param name="this">The type configuration to configure.</param>
		/// <returns>A type registration context.</returns>
		public static TypeRegistrationContext<T> Register<T>(this ITypeConfiguration<T> @this)
			=> new TypeRegistrationContext<T>(@this);

		/// <summary>
		/// Establishes a registration context for the specified member configuration.  From there, you can perform
		/// registration operations on serializers and converters for the type.
		/// </summary>
		/// <typeparam name="T">The member's containing type.</typeparam>
		/// <typeparam name="TMember">The member's value type.</typeparam>
		/// <param name="this">The member configuration to configure.</param>
		/// <returns>The member registration context.</returns>
		public static MemberRegistrationContext<T, TMember> Register<T, TMember>(
			this IMemberConfiguration<T, TMember> @this)
			=> new MemberRegistrationContext<T, TMember>(@this);

		#region Obsolete
		/// <exclude />
		[Obsolete(
			"This method is considered deprecated and will be removed in a future release.  Use IMemberConfiguration<T, TMember>.Register().Serializer().Of(Type) instead.")]
		public static IMemberConfiguration<T, TMember> Register<T, TMember>(this IMemberConfiguration<T, TMember> @this,
		                                                                    Type serializerType)
			=> @this.Register(new ActivatedSerializer(serializerType, Support<TMember>.Metadata));

		/// <exclude />
		[Obsolete(
			"This method is considered deprecated and will be removed in a future release.  Use IMemberConfiguration<T, TMember>.Register().Serializer().Of(ISerializer<T>) instead.")]
		public static IMemberConfiguration<T, TMember> Register<T, TMember>(this IMemberConfiguration<T, TMember> @this,
		                                                                    ISerializer<TMember> serializer)
			=> @this.Register(serializer.Adapt());

		/// <exclude />
		[Obsolete(
			"This method is considered deprecated and will be removed in a future release.  Use IMemberConfiguration<T, TMember>.Register().Serializer().Of(ISerializer) instead.")]
		public static IMemberConfiguration<T, TMember> Register<T, TMember>(this IMemberConfiguration<T, TMember> @this,
		                                                                    ContentModel.ISerializer serializer)
			=> @this.Root.With<CustomSerializationExtension>()
			        .Members.Apply(@this.GetMember(), serializer)
			        .Return(@this);
		/// <exclude />
		[Obsolete(
			"This method is considered deprecated and will be removed in a future release.  Use IMemberConfiguration<T, TMember>.Register().Serializer().None() instead.")]
		public static IMemberConfiguration<T, TMember> Unregister<T, TMember>(
			this IMemberConfiguration<T, TMember> @this)
			=> @this.Root.With<CustomSerializationExtension>()
			        .Members.Remove(@this.GetMember())
			        .Return(@this);

		/// <exclude />
		[Obsolete(
			"This method is considered deprecated and will be removed in a future release.  Use ITypeConfiguration<T>.Register().Serializer().Of<TSerializer>() instead.")]
		public static ITypeConfiguration<T> Register<T, TSerializer>(this IConfigurationContainer @this)
			where TSerializer : ISerializer<T>
			=> @this.Type<T>().Register().Serializer().Of<TSerializer>();

		/// <exclude />
		[Obsolete(
			"This method is considered deprecated and will be removed in a future release.  Use ITypeConfiguration<T>.Register().Serializer().Of(Type) instead.")]
		public static ITypeConfiguration<T> Register<T>(this ITypeConfiguration<T> @this, Type serializerType)
			=> @this.Register().Serializer().Of(serializerType);

		/// <exclude />
		[Obsolete(
			"This method is considered deprecated and will be removed in a future release.  Use ITypeConfiguration<T>.Register().Serializer().Of(ISerializer<T>) instead.")]
		public static ITypeConfiguration<T> Register<T>(this ITypeConfiguration<T> @this, ISerializer<T> serializer)
			=> @this.Register().Serializer().Using(serializer);

		/// <exclude />
		[Obsolete(
			"This method is considered deprecated and will be removed in a future release.  Use ITypeConfiguration<T>.Register().Serializer().Of(ISerializer) instead.")]

		public static ITypeConfiguration<T> Register<T>(this ITypeConfiguration<T> @this, ContentModel.ISerializer serializer)
			=> @this.Register().Serializer().Using(serializer);

		/// <exclude />
		[Obsolete(
			"This method is considered deprecated and will be removed in a future release.  Use ITypeConfiguration<T>.Register().Serializer().None() instead.")]
		public static ITypeConfiguration<T> Unregister<T>(this ITypeConfiguration<T> @this)
			=> @this.Register().Serializer().None();

		/// <exclude />
		[Obsolete(
			"This method is deprecated and will be removed in a future release.  Use IConfigurationContainer.Type<T>().Register().Converter().Calling() instead.")]
		public static IConfigurationContainer Register<T>(this IConfigurationContainer @this,
		                                                  Func<T, string> format,
		                                                  Func<string, T> parse)
			=> @this.Type<T>().Register().Converter().ByCalling(format, parse);

		/// <exclude />
		[Obsolete(
			"This method is deprecated and will be removed in a future release.  Use IConfigurationContainer.Type<T>().Register().Converter().Using(IConverter) instead.")]
		public static IConfigurationContainer Register<T>(this IConfigurationContainer @this, IConverter<T> converter)
			=> @this.Type<T>().Register().Converter().Using(converter);

		/// <exclude />
		[Obsolete(
			"This method is deprecated and will be removed in a future release.  Use IConfigurationContainer.Type<T>().Register().Converter().Without(IConverter) instead.")]
		public static bool Unregister<T>(this IConfigurationContainer @this, IConverter<T> converter)
			=> @this.Root.Find<ConvertersExtension>().Converters.Removing(converter);

		#endregion


	}
}