using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	sealed class ConstructorQuery : DecoratedSource<TypeInfo, IEnumerable<ConstructorInfo>>, IConstructors
	{
		readonly static DescendingConstructorQuery Query = DescendingConstructorQuery.Default;

		public ConstructorQuery(IConstructors source) : this(Query, source) {}

		public ConstructorQuery(IAlteration<IEnumerable<ConstructorInfo>> query, IConstructors source)
			: base(new AlteredSource<TypeInfo, IEnumerable<ConstructorInfo>>(query, source)) {}
	}
}