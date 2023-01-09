using System.Collections.Generic;

namespace ExtendedXmlSerializer.ExtensionModel.References;

record ReferenceResult(HashSet<object> Encountered, HashSet<object> Cyclical)
{
	protected ReferenceResult() : this(new HashSet<object>(), new HashSet<object>()) {}
}