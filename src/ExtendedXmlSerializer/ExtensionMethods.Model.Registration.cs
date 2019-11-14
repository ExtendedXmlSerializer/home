using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;
using System;

namespace ExtendedXmlSerializer
{
	public sealed class TypeSerializationRegistrationContext<T>
	{
		readonly ITypeConfiguration<T> _configuration;

		public TypeSerializationRegistrationContext(ITypeConfiguration<T> configuration)
			=> _configuration = configuration;

		public ITypeConfiguration<T> Of<TSerializer>() where TSerializer : ISerializer<T>
			=> Of(Support<TSerializer>.Key);

		public ITypeConfiguration<T> Of(Type serializerType)
			=> Using(new ActivatedSerializer(serializerType, Support<T>.Metadata));

		public ITypeConfiguration<T> Using(ISerializer<T> serializer) => Using(serializer.Adapt());

		public ITypeConfiguration<T> Using(ISerializer serializer)
			=> _configuration.Root.With<CustomSerializationExtension>()
			                 .Types.Apply(_configuration.Get(), serializer)
			                 .Return(_configuration);

		public ITypeConfiguration<T> None() => _configuration.Root.With<CustomSerializationExtension>()
		                                                     .Types.Remove(_configuration.Get())
		                                                     .Return(_configuration);
	}

	public sealed class TypeRegistrationContext<T>
	{
		readonly ITypeConfiguration<T> _configuration;

		public TypeRegistrationContext(ITypeConfiguration<T> configuration) => _configuration = configuration;

		/*public ITypeConfiguration<T> Serializer<TSerializer>() where TSerializer : ISerializer<T>
			=> Serializer(Support<TSerializer>.Key);

		public ITypeConfiguration<T> Serializer(Type serializerType)
			=> Serializer(new ActivatedSerializer(serializerType, Support<T>.Metadata));

		public ITypeConfiguration<T> Serializer(ISerializer<T> serializer) => Serializer(serializer.Adapt());

		public ITypeConfiguration<T> Serializer(ISerializer serializer)
			=> _configuration.Root.With<CustomSerializationExtension>()
			                 .Types.Apply(_configuration.Get(), serializer)
			                 .Return(_configuration);

		public ITypeConfiguration<T> ClearSerializer() => _configuration.Root.With<CustomSerializationExtension>()
		                                                                .Types.Remove(_configuration.Get())
		                                                                .Return(_configuration);*/

		public TypeSerializationRegistrationContext<T> Serializer()
			=> new TypeSerializationRegistrationContext<T>(_configuration);
	}

	// ReSharper disable once MismatchedFileName
	public static partial class ExtensionMethods
	{
		public static TypeRegistrationContext<T> Register<T>(this ITypeConfiguration<T> @this)
			=> new TypeRegistrationContext<T>(@this);

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

		/// <summary>
		/// Registers a converter for the provided type.  This defines how to deconstruct a type into a string for
		/// serialization, and to construct a string during deserialization.
		/// </summary>
		/// <typeparam name="T">The type to convert.</typeparam>
		/// <param name="this">The configuration container to configure.</param>
		/// <param name="format">The formatter to use during serialization.</param>
		/// <param name="parse">The parser to use during deserialization.</param>
		/// <returns>The configured container.</returns>
		public static IConfigurationContainer Register<T>(this IConfigurationContainer @this,
		                                                  Func<T, string> format,
		                                                  Func<string, T> parse)
			=> @this.Register<T>(new Converter<T>(parse, format));

		/// <summary>
		/// Registers a converter for the provided type.  This defines how to deconstruct a type into a string for
		/// serialization, and to construct a string during deserialization.
		/// </summary>
		/// <typeparam name="T">The type to convert.</typeparam>
		/// <param name="this">The configuration container to configure.</param>
		/// <param name="converter">The converter to register.</param>
		/// <returns>The configured container.</returns>
		public static IConfigurationContainer Register<T>(this IConfigurationContainer @this, IConverter<T> converter)
		{
			var item = converter as Converter<T> ?? Converters<T>.Default.Get(converter);
			return @this.Root.Find<ConvertersExtension>()
			            .Converters
			            .AddOrReplace(item)
			            .Return(@this);
		}

		/// <summary>
		/// Removes the registration (if any) from the container's converter registration.
		/// </summary>
		/// <typeparam name="T">The type that the converter processes.</typeparam>
		/// <param name="this">The configuration container to configure.</param>
		/// <param name="converter">The converter to remove from registration.</param>
		/// <returns>The configured container.</returns>
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