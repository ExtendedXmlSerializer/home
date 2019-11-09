using System;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Types.Sources
{
	public sealed class AllAssemblyTypes : Items<Type>
	{
		public AllAssemblyTypes(Type referenceType) : base(referenceType.Assembly.DefinedTypes.ToTypes()) {}
	}

	public sealed class AllAssemblyTypes<T> : Items<Type>
	{
		public AllAssemblyTypes() : base(new AllAssemblyTypes(typeof(T))) {}
	}
}