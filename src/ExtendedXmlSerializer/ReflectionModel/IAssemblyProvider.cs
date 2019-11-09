using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

#if CORE
#endif

namespace ExtendedXmlSerializer.ReflectionModel
{
	interface IAssemblyProvider : ISource<IEnumerable<Assembly>> {}
}