using System.Collections.Generic;

namespace ExtendedXmlSerializer.Core.Sources
{
	interface IFinish<out T> : IEnumerable<T> {}
}