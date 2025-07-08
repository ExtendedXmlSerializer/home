using ExtendedXmlSerializer.ExtensionModel.References;
using ExtendedXmlSerializer.ReflectionModel;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel
{
	static class Defaults
	{
		public static TypeInfo Reference { get; } = typeof(ReferenceIdentity).GetTypeInfo();

		public static ITypeComparer TypeComparer { get; }
			= new CompositeTypeComparer(ImplementedTypeComparer.Default,
			                            TypeIdentityComparer.Default,
			                            InheritedTypeComparer.Default);

		public static ITypeComparer SpecificTypeComparer { get; } 
			= new CompositeTypeComparer(TypeIdentityComparer.Default, InheritedTypeComparer.Default);
	}
}