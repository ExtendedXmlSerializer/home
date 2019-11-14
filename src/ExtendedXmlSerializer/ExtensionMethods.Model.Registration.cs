using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;
using System;

namespace ExtendedXmlSerializer
{
	// ReSharper disable once MismatchedFileName
	public static partial class ExtensionMethods
	{
		/// <summary>
		/// Establishes a registration context for the specified type configuration.  From there, you can perform operations
		/// on serializers and converters for the type.
		/// </summary>
		/// <typeparam name="T">The type under configuration.</typeparam>
		/// <param name="this">The type configuration to configure.</param>
		/// <returns>A type registration context.</returns>
		public static TypeRegistrationContext<T> Register<T>(this ITypeConfiguration<T> @this)
			=> new TypeRegistrationContext<T>(@this);

		public static IMemberConfiguration<T, TMember> Register<T, TMember>(this IMemberConfiguration<T, TMember> @this,
		                                                                    Type serializerType)
			=> @this.Register(new ActivatedSerializer(serializerType, Support<TMember>.Metadata));

		public static IMemberConfiguration<T, TMember> Register<T, TMember>(this IMemberConfiguration<T, TMember> @this,
		                                                                    ISerializer<TMember> serializer)
			=> @this.Register(serializer.Adapt());

		public static IMemberConfiguration<T, TMember> Register<T, TMember>(this IMemberConfiguration<T, TMember> @this,
		                                                                    ISerializer serializer)
			=> @this.Root.With<CustomSerializationExtension>()
			        .Members.Apply(@this.GetMember(), serializer)
			        .Return(@this);

		public static IMemberConfiguration<T, TMember> Unregister<T, TMember>(
			this IMemberConfiguration<T, TMember> @this)
			=> @this.Root.With<CustomSerializationExtension>()
			        .Members.Remove(@this.GetMember())
			        .Return(@this);

		#region Obsolete

		[Obsolete(
			"This method is considered deprecated and will be removed in a future release.  Use ITypeConfiguration<T>.Register().Serializer().Of<TSerializer>() instead.")]
		public static ITypeConfiguration<T> Register<T, TSerializer>(this IConfigurationContainer @this)
			where TSerializer : ISerializer<T>
			=> @this.Type<T>().Register().Serializer().Of<TSerializer>();

		[Obsolete(
			"This method is considered deprecated and will be removed in a future release.  Use ITypeConfiguration<T>.Register().Serializer().Of(Type) instead.")]
		public static ITypeConfiguration<T> Register<T>(this ITypeConfiguration<T> @this, Type serializerType)
			=> @this.Register().Serializer().Of(serializerType);

		[Obsolete(
			"This method is considered deprecated and will be removed in a future release.  Use ITypeConfiguration<T>.Register().Serializer().Of(ISerializer<T>) instead.")]
		public static ITypeConfiguration<T> Register<T>(this ITypeConfiguration<T> @this, ISerializer<T> serializer)
			=> @this.Register().Serializer().Using(serializer);

		[Obsolete(
			"This method is considered deprecated and will be removed in a future release.  Use ITypeConfiguration<T>.Register().Serializer().Of(ISerializer) instead.")]
		public static ITypeConfiguration<T> Register<T>(this ITypeConfiguration<T> @this, ISerializer serializer)
			=> @this.Register().Serializer().Using(serializer);

		[Obsolete(
			"This method is considered deprecated and will be removed in a future release.  Use ITypeConfiguration<T>.Register().Serializer().None() instead.")]
		public static ITypeConfiguration<T> Unregister<T>(this ITypeConfiguration<T> @this)
			=> @this.Register().Serializer().None();

		[Obsolete(
			"This method is deprecated and will be removed in a future release.  Use IConfigurationContainer.Type<T>().Register().Converter().Calling() instead.")]
		public static IConfigurationContainer Register<T>(this IConfigurationContainer @this,
		                                                  Func<T, string> format,
		                                                  Func<string, T> parse)
			=> @this.Type<T>().Register().Converter().ByCalling(format, parse);

		[Obsolete(
			"This method is deprecated and will be removed in a future release.  Use IConfigurationContainer.Type<T>().Register().Converter().Using(IConverter) instead.")]
		public static IConfigurationContainer Register<T>(this IConfigurationContainer @this, IConverter<T> converter)
			=> @this.Type<T>().Register().Converter().Using(converter);

		[Obsolete(
			"This method is deprecated and will be removed in a future release.  Use IConfigurationContainer.Type<T>().Register().Converter().Without(IConverter) instead.")]
		public static bool Unregister<T>(this IConfigurationContainer @this, IConverter<T> converter)
			=> @this.Root.Find<ConvertersExtension>().Converters.Removing(converter);

		#endregion


	}
}