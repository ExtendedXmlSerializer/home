using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtendedXmlSerializer.ExtensionModel.Types.Sources
{
	sealed class TypesInSameNamespace : Items<Type>
	{
		public TypesInSameNamespace(Type referenceType, IEnumerable<Type> candidates)
			: base(candidates.Where(x => x.Namespace == referenceType.Namespace)) {}
	}
}