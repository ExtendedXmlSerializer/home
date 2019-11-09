using System;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Types.Sources
{
	public sealed class PublicNestedTypes<T> : Items<Type>
	{
		public PublicNestedTypes() : base(new PublicNestedTypes(typeof(T))) {}
	}

	public sealed class PublicNestedTypes : Items<Type>
	{
		public PublicNestedTypes(Type referenceType) : base(referenceType.GetNestedTypes()) {}
	}
}