using System;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Types.Sources
{
	public sealed class PublicAssemblyTypes<T> : Items<Type>
	{
		public PublicAssemblyTypes() : base(new PublicAssemblyTypes(typeof(T))) {}
	}

	public sealed class PublicAssemblyTypes : Items<Type>
	{
		public PublicAssemblyTypes(Type referenceType) : base(referenceType.Assembly.ExportedTypes) {}
	}
}