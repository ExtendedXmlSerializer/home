using System.Collections.Immutable;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	/// <summary>
	/// A composite type comparer that uses a collection of comparers to return a result.  If any of the comparers return
	/// true then the result is true.
	/// </summary>
	public sealed class CompositeTypeComparer : ITypeComparer
	{
		readonly ImmutableArray<ITypeComparer> _comparers;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="comparers"></param>
		public CompositeTypeComparer(params ITypeComparer[] comparers) : this(comparers.ToImmutableArray()) {}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="comparers"></param>
		public CompositeTypeComparer(ImmutableArray<ITypeComparer> comparers) => _comparers = comparers;

		/// <inheritdoc />
		public bool Equals(TypeInfo x, TypeInfo y)
		{
			var length = _comparers.Length;
			for (var i = 0; i < length; i++)
			{
				if (_comparers[i].Equals(x, y))
				{
					return true;
				}
			}

			return false;
		}

		/// <inheritdoc />
		public int GetHashCode(TypeInfo obj) => 0;
	}
}