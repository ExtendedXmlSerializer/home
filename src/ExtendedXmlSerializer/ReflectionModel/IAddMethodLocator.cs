using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	interface IAddMethodLocator
	{
		MethodInfo Locate(TypeInfo type, TypeInfo elementType);
	}
}