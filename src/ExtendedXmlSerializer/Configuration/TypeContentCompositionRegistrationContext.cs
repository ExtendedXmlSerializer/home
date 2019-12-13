using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using ISerializer = ExtendedXmlSerializer.ContentModel.ISerializer;

namespace ExtendedXmlSerializer.Configuration
{
	/// <summary>
	/// Provides a context for registering content composers for a particular type.
	/// </summary>
	/// <typeparam name="T">The type under configuration.</typeparam>
	public sealed class TypeContentCompositionRegistrationContext<T>
	{
		readonly ITypeConfiguration<T> _configuration;

		/// <inheritdoc />
		public TypeContentCompositionRegistrationContext(ITypeConfiguration<T> configuration)
			=> _configuration = configuration;

		/// <summary>
		/// Registers a content serializer composer of the specified type.
		/// </summary>
		/// <typeparam name="TComposer">The type of the content composer to register.</typeparam>
		/// <returns>The configured type configuration.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/home/issues/264#issuecomment-531491807"/>
		public ITypeConfiguration<T> Of<TComposer>() where TComposer : ISerializerComposer
			=> Of(Support<TComposer>.Key);

		/// <summary>
		/// Registers a content serializer composer of the specified type.
		/// </summary>
		/// <param name="composerType">The type that implements <see cref="ISerializerComposer"/> of the content composer to
		/// register.</param>
		/// <returns>The configured type configuration.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/home/issues/264#issuecomment-531491807"/>
		public ITypeConfiguration<T> Of(Type composerType)
			=> _configuration.Root.With<RegisteredCompositionExtension>()
			                 .Apply(Support<T>.Metadata, composerType)
			                 .Return(_configuration);


		/// <summary>
		/// Used to alter a serializer whenever one is created for a specific type.  This allows the scenario of decorating
		/// a serializer to override or monitor serialization and/or deserialization.
		/// </summary>
		/// <param name="compose">The delegate that defines how to create a content serializer.</param>
		/// <returns>The configured type configuration.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/home/issues/264#issuecomment-531491807"/>
		public ITypeConfiguration<T> ByCalling(Func<ISerializer<T>, ISerializer<T>> compose)
			=> Using(new SerializerComposer<T>(compose));

		/// <summary>
		/// Used to alter a serializer whenever one is created for a specific type.  This allows the scenario of decorating
		/// a serializer to override or monitor serialization and/or deserialization.  This override accepts a generalized
		/// serializer delegate.
		/// </summary>
		/// <param name="compose">The delegate that defines how to create a content serializer.</param>
		/// <returns>The configured type configuration.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/home/issues/264#issuecomment-531491807"/>
		public ITypeConfiguration<T> ByCalling(Func<ISerializer, ISerializer> compose)
			=> Using(new SerializerComposer(compose));

		/// <summary>
		/// Used to alter a serializer whenever one is created for a specific type.  This allows the scenario of decorating a
		/// serializer to override or monitor serialization and/or deserialization.  This override accepts an
		/// <see cref="ISerializerComposer"/> that performs the alteration on the created serializer.
		/// </summary>
		/// <param name="composer">The serializer composer to register.</param>
		/// <returns>The configured type configuration.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/home/issues/264#issuecomment-531491807"/>
		public ITypeConfiguration<T> Using(ISerializerComposer composer)
			=> _configuration.Root.With<RegisteredCompositionExtension>()
			                 .Apply(Support<T>.Metadata, composer)
			                 .Return(_configuration);

		/// <summary>
		/// Clears any registered content serializer composers for the type under configuration.
		/// </summary>
		/// <returns>The configured type configuration.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/home/issues/264#issuecomment-531491807"/>
		public ITypeConfiguration<T> None() => _configuration.Root.With<RegisteredCompositionExtension>()
		                                                     .Apply(_configuration.Get())
		                                                     .Return(_configuration);
	}
}