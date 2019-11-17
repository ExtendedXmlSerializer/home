using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using System;

namespace ExtendedXmlSerializer.ExtensionModel.Types.Sources
{
	/// <summary>
	/// Iterates through all private and public types found in the assembly of the provided reference type.
	/// </summary>
	public sealed class AllAssemblyTypes : Items<Type>
	{
		/// <summary>
		/// Creates a new instance from the provided reference type.
		/// </summary>
		/// <param name="referenceType">The reference type of which to query.</param>
		public AllAssemblyTypes(Type referenceType) : base(referenceType.Assembly.DefinedTypes.ToTypes()) {}
	}

	/// <summary>
	/// Iterates through all private and public types found in the assembly of the provided reference type.
	/// </summary>
	/// <typeparam name="T">The reference type of which to query.</typeparam>
	public sealed class AllAssemblyTypes<T> : Items<Type>
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public AllAssemblyTypes() : base(new AllAssemblyTypes(typeof(T))) {}
	}
}