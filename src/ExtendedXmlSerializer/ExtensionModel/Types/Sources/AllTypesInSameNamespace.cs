using System;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Types.Sources
{
	public sealed class AllTypesInSameNamespace<T> : Items<Type>
	{
		public AllTypesInSameNamespace() : base(new AllTypesInSameNamespace(typeof(T))) {}
	}

	public sealed class AllTypesInSameNamespace : Items<Type>
	{
		public AllTypesInSameNamespace(Type referenceType) : base(new TypesInSameNamespace(referenceType,
		                                                                                   new
			                                                                                   AllAssemblyTypes(referenceType))) {}
	}
}