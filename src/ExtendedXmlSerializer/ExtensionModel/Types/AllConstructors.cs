using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	sealed class AllConstructors : IConstructors
	{
		readonly IConstructors _constructors;

		public AllConstructors(IConstructors constructors) => _constructors = constructors;

		public IEnumerable<ConstructorInfo> Get(TypeInfo parameter)
			=> parameter.DeclaredConstructors.Where(x => !x.IsStatic)
			            .Union(_constructors.Get(parameter));
	}
}