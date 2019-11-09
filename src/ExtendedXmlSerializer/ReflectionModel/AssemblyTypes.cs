using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	class AssemblyTypes : IAssemblyTypes
	{
		public static AssemblyTypes Default { get; } = new AssemblyTypes();

		AssemblyTypes() {}

		public IEnumerable<TypeInfo> Get(Assembly parameter) => parameter.DefinedTypes;
	}
}