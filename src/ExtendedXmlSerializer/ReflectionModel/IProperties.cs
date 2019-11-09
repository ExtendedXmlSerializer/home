using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	interface IProperties : IParameterizedSource<TypeInfo, IEnumerable<PropertyInfo>> {}
}