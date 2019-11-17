using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Linq;

namespace ExtendedXmlSerializer.Configuration
{
	/// <summary>
	/// Establishes context that enables converter registration operations to be performed on the subject type
	/// configuration.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class TypeConverterRegistrationContext<T>
	{
		readonly ITypeConfiguration<T> _configuration;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="configuration">The type configuration under configuration.</param>
		public TypeConverterRegistrationContext(ITypeConfiguration<T> configuration) => _configuration = configuration;

		/// <summary>
		/// Registers a converter for the provided type.  This defines how to deconstruct an instance of the currently
		/// configured type into a string for serialization, and to construct an instance of the currently configured type
		/// from a string during deserialization.
		/// </summary>
		/// <param name="format">The formatter to use during serialization.</param>
		/// <param name="parse">The parser to use during deserialization.</param>
		/// <returns>The configured type container.</returns>
		public ITypeConfiguration<T> ByCalling(Func<T, string> format, Func<string, T> parse)
			=> Using(new Converter<T>(parse, format));

		/// <summary>
		/// Registers a converter for the provided type.  This defines how to deconstruct an instance of the currently
		/// configured type into a string for serialization, and to construct an instance of the currently configured type
		/// from a string during deserialization.
		/// </summary>
		/// <typeparam name="T">The type to convert.</typeparam>
		/// <param name="converter">The converter to register.</param>
		/// <returns>The configured type container.</returns>
		public ITypeConfiguration<T> Using(IConverter<T> converter)
		{
			var item = converter as Converter<T> ?? Converters.Default.Get(converter);
			return _configuration.Root.Find<ConvertersExtension>()
			                     .Converters
			                     .AddOrReplace(item)
			                     .Return(_configuration);
		}

		/// <summary>
		/// Removes the registration (if any) from the container's converter registration.
		/// </summary>
		/// <param name="converter">The converter to remove from registration.</param>
		/// <returns>The configured type container.</returns>
		public ITypeConfiguration<T> Without(IConverter converter) => _configuration.Root.Find<ConvertersExtension>()
		                                                                            .Converters.Remove(converter)
		                                                                            .Return(_configuration);

		/// <summary>
		/// Removes the registration (if any) from the container's converter registration.
		/// </summary>
		/// <param name="converter">The converter to remove from registration.</param>
		/// <returns>The configured type container.</returns>
		public ITypeConfiguration<T> Without(IConverter<T> converter) => _configuration.Root.Find<ConvertersExtension>()
		                                                                               .Converters.Removing(converter)
		                                                                               .Return(_configuration);

		/// <summary>
		/// Removes all registered converters that work with the currently configured type.
		/// </summary>
		/// <returns>The configured type configuration.</returns>
		public ITypeConfiguration<T> None()
		{
			foreach (var converter in _configuration.Root.Find<ConvertersExtension>()
			                                        .Converters.ToArray()
			                                        .Where(x => x.IsSatisfiedBy(Support<T>.Metadata)))
			{
				Without(converter);
			}

			return _configuration;
		}

		sealed class Converters : ReferenceCache<IConverter<T>, IConverter<T>>
		{
			public static Converters Default { get; } = new Converters();

			Converters() : base(key => new Converter<T>(key, key.Parse, key.Format)) {}
		}
	}
}