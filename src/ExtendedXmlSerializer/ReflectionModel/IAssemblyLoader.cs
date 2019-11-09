using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	interface IAssemblyLoader : IParameterizedSource<string, Assembly> {}
}