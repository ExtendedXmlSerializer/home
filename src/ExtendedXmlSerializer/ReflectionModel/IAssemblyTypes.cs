using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	interface IAssemblyTypes : IParameterizedSource<Assembly, IEnumerable<TypeInfo>> {}
}