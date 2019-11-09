using System.Collections;
using System.Collections.Generic;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class TrackedLists : Cache<object, Stack<IList>>, ITrackedLists
	{
		public TrackedLists() : base(_ => new Stack<IList>()) {}
	}
}