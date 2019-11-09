using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	sealed class QueriedConstructors : Cache<TypeInfo, ConstructorInfo>, IQueriedConstructors
	{
		public QueriedConstructors(IValidConstructorSpecification specification, IConstructors constructors)
			: base(new ConstructorLocator(specification, new ConstructorQuery(constructors)).Get) {}
	}
}