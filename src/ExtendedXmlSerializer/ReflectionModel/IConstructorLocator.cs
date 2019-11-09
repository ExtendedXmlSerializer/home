using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	interface IConstructorLocator : IParameterizedSource<TypeInfo, ConstructorInfo> {}
}