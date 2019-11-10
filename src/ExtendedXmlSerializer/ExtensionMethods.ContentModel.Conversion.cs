using ExtendedXmlSerializer.ContentModel.Conversion;

namespace ExtendedXmlSerializer
{
	// ReSharper disable once MismatchedFileName
	public static partial class ExtensionMethods
	{
		public static IConverter<T?> Structured<T>(this IConverter<T> @this) where T : struct
			=> new StructureConverter<T>(@this);

		public static IConverter Adapt<T>(this IConverter<T> @this) => new Converter<T>(@this.Parse, @this.Format);
	}
}
