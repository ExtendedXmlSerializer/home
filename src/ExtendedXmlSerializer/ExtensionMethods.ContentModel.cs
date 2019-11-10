using ExtendedXmlSerializer.ContentModel;

namespace ExtendedXmlSerializer
{
	// ReSharper disable once MismatchedFileName
	public static partial class ExtensionMethods
	{
		public static ISerializer<T> For<T>(this ISerializer @this) => @this as ISerializer<T> ??
		                                                               new GeneralSerializerAdapter<T>(@this);

		public static ISerializer Adapt<T>(this ISerializer<T> @this) => @this as ISerializer ??
		                                                                 new GenericSerializerAdapter<T>(@this);

		public static IWriter Adapt<T>(this IWriter<T> @this) => @this as IWriter ?? new GenericWriterAdapter<T>(@this);
	}
}
