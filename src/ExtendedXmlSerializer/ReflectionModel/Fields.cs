using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class Fields : IFields
	{
		public static Fields Default { get; } = new Fields();

		Fields() {}

		public IEnumerable<FieldInfo> Get(TypeInfo parameter) => parameter.GetRuntimeFields();
	}
}