using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class AssemblyPublicTypes : IAssemblyTypes
	{
		public static AssemblyPublicTypes Default { get; } = new AssemblyPublicTypes();

		AssemblyPublicTypes() {}

		public IEnumerable<TypeInfo> Get(Assembly parameter) => parameter.ExportedTypes.YieldMetadata();
	}
}