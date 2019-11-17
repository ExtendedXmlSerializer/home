using ExtendedXmlSerializer.Core.Sources;
using System;

namespace ExtendedXmlSerializer.ExtensionModel.Types.Sources
{
	/// <summary>
	/// Iterates through all public types found in the namespace of the provided reference type.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class PublicTypesInSameNamespace<T> : Items<Type>
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public PublicTypesInSameNamespace() : base(new PublicTypesInSameNamespace(typeof(T))) {}
	}

	/// <summary>
	/// Iterates through all public types found in the namespace of the provided reference type.
	/// </summary>
	public sealed class PublicTypesInSameNamespace : Items<Type>
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="referenceType">The reference type to query.</param>
		public PublicTypesInSameNamespace(Type referenceType)
			: base(new TypesInSameNamespace(referenceType, new AllAssemblyTypes(referenceType))) {}
	}
}