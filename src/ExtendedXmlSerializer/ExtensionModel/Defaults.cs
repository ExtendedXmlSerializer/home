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

		public static IMemberComparer MemberComparer { get; } = new MemberComparer(TypeComparer);
	}
}