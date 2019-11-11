namespace ExtendedXmlSerializer
{
	// ReSharper disable once MismatchedFileName
	public static partial class ExtensionMethods
	{
		/// <summary>Convenience method used for fluent-type methods.</summary>
		/// <typeparam name="T">The type of the calling instance.</typeparam>
		/// <typeparam name="TOut">The result type.</typeparam>
		/// <param name="_">Not used.</param>
		/// <param name="result">The result.</param>
		/// <returns>TOut.</returns>
		public static TOut Return<T, TOut>(this T _, TOut result) => result;
	}
}
