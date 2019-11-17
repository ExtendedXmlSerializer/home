using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Types.Sources
{
	/// <summary>
	/// Iterates through all private and public nested types found in the provided reference type.
	/// </summary>
	/// <typeparam name="T">The reference type to query.</typeparam>
	public sealed class NestedTypes<T> : Items<Type>
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public NestedTypes() : base(new NestedTypes(typeof(T))) {}
	}

	/// <summary>
	/// Iterates through all private and public nested types found in the provided reference type.
	/// </summary>
	public sealed class NestedTypes : Items<Type>
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="referenceType">The reference type to query.</param>
		public NestedTypes(Type referenceType) : base(referenceType.GetTypeInfo().DeclaredNestedTypes.ToTypes()) {}
	}
}