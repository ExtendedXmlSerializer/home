using ExtendedXmlSerializer.Core.Sources;
using System;

namespace ExtendedXmlSerializer.ExtensionModel.Types.Sources
{
	/// <summary>
	/// Iterates through all public nested types found in the provided reference type.
	/// </summary>
	/// <typeparam name="T">The type to query.</typeparam>
	public sealed class PublicNestedTypes<T> : Items<Type>
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public PublicNestedTypes() : base(new PublicNestedTypes(typeof(T))) {}
	}

	/// <summary>
	/// Iterates through all public nested types found in the provided reference type.
	/// </summary>
	public sealed class PublicNestedTypes : Items<Type>
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="referenceType">The reference type to query.</param>
		public PublicNestedTypes(Type referenceType) : base(referenceType.GetNestedTypes()) {}
	}
}