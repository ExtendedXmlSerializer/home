using System.Collections.Generic;

namespace ExtendedXmlSerializer.Core.Sources
{
	/// <inheritdoc />
	public class CompositeAlteration<T> : IAlteration<T>
	{
		readonly IEnumerable<IAlteration<T>> _alterations;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="alterations"></param>
		public CompositeAlteration(IEnumerable<IAlteration<T>> alterations) => _alterations = alterations;

		/// <inheritdoc />
		public T Get(T parameter) => _alterations.Alter(parameter);
	}
}