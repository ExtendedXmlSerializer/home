namespace ExtendedXmlSerializer
{
	// ReSharper disable once MismatchedFileName
	public static partial class ExtensionMethods
	{
		/// <summary>
		/// Convenience method for fluent-type methods.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="_"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public static TOut Return<T, TOut>(this T _, TOut result) => result;
	}
}
