using System;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Types.Sources
{
	public sealed class PublicTypesInSameNamespace<T> : Items<Type>
	{
		public PublicTypesInSameNamespace() : base(new PublicTypesInSameNamespace(typeof(T))) {}
	}

	public sealed class PublicTypesInSameNamespace : Items<Type>
	{
		public PublicTypesInSameNamespace(Type referenceType) : base(new TypesInSameNamespace(referenceType,
		                                                                                      new
			                                                                                      AllAssemblyTypes(referenceType))) {}
	}
}