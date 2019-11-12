using ExtendedXmlSerializer.Core.Sources;
using System.Reflection;

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

		/// <summary>
		/// Convenience method for objects that implement several <see cref="ISource{T}"/> to get its type-based contents specifically.
		/// </summary>
		/// <param name="this">The implementing source.</param>
		/// <returns>TypeInfo.</returns>
		public static TypeInfo GetType(this ISource<TypeInfo> @this) => @this.Get();

		/// <summary>
		/// Convenience method for objects that implement several <see cref="ISource{T}"/> to get its member-based contents specifically.
		/// </summary>
		/// <param name="this">The implementing source.</param>
		/// <returns>MemberInfo.</returns>
		public static MemberInfo GetMember(this ISource<MemberInfo> @this) => @this.Get();
	}
}
