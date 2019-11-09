using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class Constructors : IConstructors
	{
		public static Constructors Default { get; } = new Constructors();

		Constructors() {}

		public IEnumerable<ConstructorInfo> Get(TypeInfo parameter) => parameter.GetConstructors();
	}
}