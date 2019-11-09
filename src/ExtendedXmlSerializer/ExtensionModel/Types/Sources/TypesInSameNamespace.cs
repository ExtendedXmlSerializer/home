using System;
using System.Collections.Generic;
using System.Linq;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Types.Sources
{
	public sealed class TypesInSameNamespace : Items<Type>
	{
		public TypesInSameNamespace(Type referenceType, IEnumerable<Type> candidates) :
			base(candidates.Where(x => x.Namespace == referenceType.Namespace)) {}
	}
}