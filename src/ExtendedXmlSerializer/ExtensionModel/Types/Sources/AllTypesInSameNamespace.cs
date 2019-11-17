using ExtendedXmlSerializer.Core.Sources;
using System;

namespace ExtendedXmlSerializer.ExtensionModel.Types.Sources
{
	/// <summary>
	/// Iterates through all private and public types found in the namespace of the provided reference type.
	/// </summary>
	/// <typeparam name="T">The reference type to query.</typeparam>
	public sealed class AllTypesInSameNamespace<T> : Items<Type>
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public AllTypesInSameNamespace() : base(new AllTypesInSameNamespace(typeof(T))) {}
	}

	/// <summary>
	/// Iterates through all private and public types found in the namespace of the provided reference type.
	/// </summary>
	public sealed class AllTypesInSameNamespace : Items<Type>
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="referenceType">The reference type to query.</param>
		public AllTypesInSameNamespace(Type referenceType)
			: base(new TypesInSameNamespace(referenceType, new AllAssemblyTypes(referenceType))) {}
	}
}