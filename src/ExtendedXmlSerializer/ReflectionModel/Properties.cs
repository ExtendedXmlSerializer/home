using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	class Properties : IProperties
	{
		public static Properties Default { get; } = new Properties();

		Properties() {}

		public IEnumerable<PropertyInfo> Get(TypeInfo parameter) => parameter.GetProperties()
		                                                                     .GroupBy(x => x.Name)
		                                                                     .Select(x => x.First());
	}
}