using ExtendedXmlSerializer.Core.Sources;
using System;

namespace ExtendedXmlSerializer.ExtensionModel.Types.Sources
{
	/// <summary>
	/// Iterates through all public types found in the assembly of the provided reference type.
	/// </summary>
	/// <typeparam name="T">The reference type to query.</typeparam>
	public sealed class PublicAssemblyTypes<T> : Items<Type>
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public PublicAssemblyTypes() : base(new PublicAssemblyTypes(typeof(T))) {}
	}

	/// <summary>
	/// Iterates through all public types found in the assembly of the provided reference type.
	/// </summary>
	public sealed class PublicAssemblyTypes : Items<Type>
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="referenceType">The reference type to query.</param>
		public PublicAssemblyTypes(Type referenceType) : base(referenceType.Assembly.ExportedTypes) {}
	}
}