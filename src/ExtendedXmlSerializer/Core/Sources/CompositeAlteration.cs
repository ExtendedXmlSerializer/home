using System.Collections.Generic;

namespace ExtendedXmlSerializer.Core.Sources
{
	public class CompositeAlteration<T> : IAlteration<T>
	{
		readonly IEnumerable<IAlteration<T>> _alterations;

		public CompositeAlteration(IEnumerable<IAlteration<T>> alterations) => _alterations = alterations;

		public T Get(T parameter) => _alterations.Alter(parameter);
	}
}