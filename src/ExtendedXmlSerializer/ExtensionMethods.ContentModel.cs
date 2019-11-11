using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Conversion;

namespace ExtendedXmlSerializer
{
	// ReSharper disable once MismatchedFileName
	public static partial class ExtensionMethods
	{
		/// <summary>
		/// Given a generalized serializer, create a type-specific serializer that handles the specified type.  This is typically used for simple casting to and from a general object type and should be handled with care as an incorrect type will throw errors.
		/// </summary>
		/// <typeparam name="T">The type to use for serialization.</typeparam>
		/// <param name="this">The this.</param>
		/// <returns>ISerializer&lt;T&gt;.</returns>
		public static ISerializer<T> For<T>(this ISerializer @this) => @this as ISerializer<T> ??
		                                                               new GeneralSerializerAdapter<T>(@this);

		/// <summary>
		/// Given a type-specific serializer, create a generalized serializer that serializes and deserializes in terms of a general <see cref="System.Object"/>.
		/// </summary>
		/// <typeparam name="T">The type that the given serializer uses.</typeparam>
		/// <param name="this">The serializer used to create a new serializer.</param>
		public static ISerializer Adapt<T>(this ISerializer<T> @this) => @this as ISerializer ??
		                                                                 new GenericSerializerAdapter<T>(@this);

		/// <summary>
		/// Given a type-specific writer, create a generalized writer that writers in terms of a general <see cref="System.Object"/>.
		/// </summary>
		/// <typeparam name="T">The type that the writer is used to write.</typeparam>
		/// <param name="this">The writer instance which to base the new writer.</param>
		/// <returns>The generalized writer.</returns>
		public static IWriter Adapt<T>(this IWriter<T> @this) => @this as IWriter ?? new GenericWriterAdapter<T>(@this);

		/// <summary>
		/// Given a struct-specific converter, creates its nullable equivalent.
		/// </summary>
		/// <typeparam name="T">The struct type of the converter.</typeparam>
		/// <param name="this">The converter upon which to base the new converter.</param>
		/// <returns>IConverter&lt;System.Nullable&lt;T&gt;&gt;.</returns>
		public static IConverter<T?> Structured<T>(this IConverter<T> @this) where T : struct
			=> new StructureConverter<T>(@this);

		/// <summary>
		/// Given a type-specific converter, create a generalized converter that serializes and deserializes in terms of a general <see cref="System.Object"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="this">The type-specific converter upon which to base the new generalized converter.</param>
		/// <returns>A generalized IConverter.</returns>
		public static IConverter Adapt<T>(this IConverter<T> @this) => new Converter<T>(@this.Parse, @this.Format);
	}
}
