using System.Collections;
using System.Collections.Generic;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	interface ITrackedLists : IParameterizedSource<object, Stack<IList>> {}
}