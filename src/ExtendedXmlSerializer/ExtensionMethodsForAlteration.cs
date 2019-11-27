using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ReflectionModel;
using System;

namespace ExtendedXmlSerializer
{
	/// <summary>
	/// Extension methods that assist or enable functionality for altering content produced (or read in) by the container,
	/// its types, and/or its type's members.
	/// </summary>
	public static class ExtensionMethodsForAlteration
	{
		/// <summary>
		/// Used to alter an instance of the configured result type whenever it is encountered during the serialization
		/// process.  This can be used in scenarios where it is desired to know when an instance of a particular type is
		/// emitted (for logging purposes, etc.) or, more generally, to alter it in some way (scrubbing data, etc)
		/// before it is written to the external stream.  You can consider this as a value interception of
		/// the serialization pipeline.
		/// </summary>
		/// <typeparam name="T">The type under configuration.</typeparam>
		/// <param name="this">The type configuration to configure.</param>
		/// <param name="write">The alteration delegate to invoke during writing.</param>
		/// <returns>The configured type configuration.</returns>
		public static ITypeConfiguration<T> Alter<T>(this ITypeConfiguration<T> @this, Func<T, T> write)
			=> @this.Alter(Self.Of<T>(), write);

		/// <summary>
		/// Used to alter an instance of the configured result type whenever it is encountered during the serialization or
		/// deserialization process.  This can be used in scenarios where it is desired to know when an instance of a
		/// particular type is emitted or read (for logging purposes, etc.) or, more generally, to alter it in some way
		/// (scrubbing data, etc) before it is written to the external stream or read into memory.  You can consider this as a
		/// value interception of the serialization/deserialization pipeline.
		/// </summary>
		/// <typeparam name="T">The type under configuration.</typeparam>
		/// <param name="this">The type configuration to configure.</param>
		/// <param name="read">The alteration delegate to invoke during reading.</param>
		/// <param name="write">The alteration delegate to invoke during writing.</param>
		/// <returns>The configured type configuration.</returns>
		public static ITypeConfiguration<T> Alter<T>(this ITypeConfiguration<T> @this, Func<T, T> read,
		                                             Func<T, T> write)
			=> @this.Alter(new DelegatedAlteration<T>(read), new DelegatedAlteration<T>(write));

		/// <summary>
		/// Used to alter an instance of the configured result type whenever it is encountered during the serialization or
		/// deserialization process.  This can be used in scenarios where it is desired to know when an instance of a
		/// particular type is emitted or read (for logging purposes, etc.) or, more generally, to alter it in some way
		/// (scrubbing data, etc) before it is written to the external stream or read into memory.  You can consider this as a
		/// value interception of the serialization/deserialization pipeline.
		/// </summary>
		/// <typeparam name="T">The type under configuration.</typeparam>
		/// <param name="this">The type configuration to configure.</param>
		/// <param name="read">The alteration to apply during reading.</param>
		/// <param name="write">The alteration to apply during writing.</param>
		/// <returns>The configured type configuration.</returns>
		public static ITypeConfiguration<T> Alter<T>(this ITypeConfiguration<T> @this, IAlteration<T> read,
		                                             IAlteration<T> write)
			=> @this.Root.With<AlteredContentExtension>()
			        .Types.Apply(Support<T>.Metadata, new ContentAlteration(read.Adapt(), write.Adapt()))
			        .Return(@this);

		/// <summary>
		/// Used to alter the value of a member whenever it is encountered during the serialization process.  This can be used
		/// in scenarios where it is desired to know when a the value of a member is emitted (for logging purposes,
		/// etc.) or, more generally, to alter it in some way (scrubbing data, etc) before it is written to the external
		/// stream.  You can consider this as a member value interception of the serialization pipeline.
		/// </summary>
		/// <typeparam name="T">The containing type under configuration.</typeparam>
		/// <typeparam name="TMember">The member's value type.</typeparam>
		/// <param name="this">The member configuration under configuration.</param>
		/// <param name="write">The alteration delegate to invoke on the member value when it is written.</param>
		/// <returns>The configured member configuration.</returns>
		public static IMemberConfiguration<T, TMember> Alter<T, TMember>(this IMemberConfiguration<T, TMember> @this,
		                                                                 Func<TMember, TMember> write)
			=> @this.Alter(Self.Of<TMember>(), write);

		/// <summary>
		/// Used to alter the value of a member whenever it is encountered during the serialization or deserialization
		/// process.  This can be used in scenarios where it is desired to know when a the value of a member is emitted or
		/// read (for logging purposes, etc.) or, more generally, to alter it in some way (scrubbing data, etc) before it is
		/// written to the external stream or read into memory.  You can consider this as a member value interception of the
		/// serialization/deserialization pipeline.
		/// </summary>
		/// <typeparam name="T">The containing type under configuration.</typeparam>
		/// <typeparam name="TMember">The member's value type.</typeparam>
		/// <param name="this">The member configuration under configuration.</param>
		/// <param name="read">The alteration delegate to invoke on the member value when it is read.</param>
		/// <param name="write">The alteration delegate to invoke on the member value when it is written.</param>
		/// <returns>The configured member configuration.</returns>
		public static IMemberConfiguration<T, TMember> Alter<T, TMember>(this IMemberConfiguration<T, TMember> @this,
		                                                                 Func<TMember, TMember> read,
		                                                                 Func<TMember, TMember> write)
			=> @this.Alter(new DelegatedAlteration<TMember>(read), new DelegatedAlteration<TMember>(write));

		/// <summary>
		/// Used to alter the value of a member whenever it is encountered during the serialization or deserialization
		/// process.  This can be used in scenarios where it is desired to know when a the value of a member is emitted or
		/// read (for logging purposes, etc.) or, more generally, to alter it in some way (scrubbing data, etc) before it is
		/// written to the external stream or read into memory.  You can consider this as a member value interception of the
		/// serialization/deserialization pipeline.
		/// </summary>
		/// <typeparam name="T">The containing type under configuration.</typeparam>
		/// <typeparam name="TMember">The member's value type.</typeparam>
		/// <param name="this">The member configuration under configuration.</param>
		/// <param name="read">The alteration to apply to the member value when it is read.</param>
		/// <param name="write">The alteration to apply to the member value when it is written.</param>
		/// <returns>The configured member configuration.</returns>
		public static IMemberConfiguration<T, TMember> Alter<T, TMember>(this IMemberConfiguration<T, TMember> @this,
		                                                                 IAlteration<TMember> read,
		                                                                 IAlteration<TMember> write)
			=> @this.Root.With<AlteredContentExtension>()
			        .Members.Apply(@this.GetMember(), new ContentAlteration(read.Adapt(), write.Adapt()))
			        .Return(@this);

		/// <summary>
		/// Used to alter a serializer whenever one is created for a specific type.  This allows the scenario of decorating
		/// a serializer to override or monitor serialization and/or deserialization.
		/// </summary>
		/// <typeparam name="T">The type that the serializer processes.</typeparam>
		/// <param name="this">The type configuration to configure.</param>
		/// <param name="compose">The delegate used to alterate the created serializer.</param>
		/// <returns>The configured type configuration.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/home/issues/264#issuecomment-531491807"/>
		public static ITypeConfiguration<T> RegisterContentComposition<T>(this ITypeConfiguration<T> @this,
		                                                                  Func<ISerializer<T>, ISerializer<T>> compose)
			=> @this.RegisterContentComposition(new SerializerComposer<T>(compose).Get);

		/// <summary>
		/// Used to alter a serializer whenever one is created for a specific type.  This allows the scenario of decorating
		/// a serializer to override or monitor serialization and/or deserialization.  This override accepts a generalized
		/// serializer delegate.
		/// </summary>
		/// <typeparam name="T">The type that the serializer processes.</typeparam>
		/// <param name="this">The type configuration to configure.</param>
		/// <param name="compose">The delegate used to alterate the created serializer.</param>
		/// <returns>The configured type configuration.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/home/issues/264#issuecomment-531491807"/>
		public static ITypeConfiguration<T> RegisterContentComposition<T>(this ITypeConfiguration<T> @this,
		                                                                  Func<ISerializer, ISerializer> compose)
			=> @this.RegisterContentComposition(new SerializerComposer(compose));

		/// <summary>
		/// Used to alter a serializer whenever one is created for a specific type.  This allows the scenario of decorating a
		/// serializer to override or monitor serialization and/or deserialization.  This override accepts an
		/// <see cref="ISerializerComposer"/> that performs the alteration on the created serializer.
		/// </summary>
		/// <typeparam name="T">The type that the serializer processes.</typeparam>
		/// <param name="this">The type configuration to configure.</param>
		/// <param name="composer">The composer that is used to alter the serializer upon creation.</param>
		/// <returns>The configured type configuration.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/home/issues/264#issuecomment-531491807"/>
		public static ITypeConfiguration<T> RegisterContentComposition<T>(this ITypeConfiguration<T> @this,
		                                                                  ISerializerComposer composer)
			=> @this.Root.With<RegisteredCompositionExtension>()
			        .Apply(Support<T>.Metadata, composer)
			        .Return(@this);

		/// <summary>
		/// Provides a way to alter converters when they are accessed by the serializer.  This provides a mechanism to
		/// decorate converters.  Alterations only occur once per converter per serializer.
		/// </summary>
		/// <param name="this">The container to configure.</param>
		/// <param name="alteration">The alteration to perform on each converter when it is accessed by the serializer.</param>
		/// <returns>The configured container.</returns>
		public static IConfigurationContainer Alter(this IConfigurationContainer @this,
		                                            IAlteration<IConverter> alteration)
			=> @this.Root.With<ConverterAlterationsExtension>()
			        .Alterations.Apply(alteration)
			        .Return(@this);
	}
}
