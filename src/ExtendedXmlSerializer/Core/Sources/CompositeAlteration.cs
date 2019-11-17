using System.Collections.Generic;

namespace ExtendedXmlSerializer.Core.Sources
{
	/// <exclude />
	public class CompositeAlteration<T> : IAlteration<T>
	{
		readonly IEnumerable<IAlteration<T>> _alterations;

		/// <exclude />
		public CompositeAlteration(IEnumerable<IAlteration<T>> alterations) => _alterations = alterations;

		/// <inheritdoc />
		public T Get(T parameter) => _alterations.Alter(parameter);
	}
}