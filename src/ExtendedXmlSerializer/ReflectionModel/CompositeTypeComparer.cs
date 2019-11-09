using System.Collections.Immutable;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class CompositeTypeComparer : ITypeComparer
	{
		readonly ImmutableArray<ITypeComparer> _comparers;

		public CompositeTypeComparer(params ITypeComparer[] comparers) : this(comparers.ToImmutableArray()) {}

		public CompositeTypeComparer(ImmutableArray<ITypeComparer> comparers) => _comparers = comparers;

		public bool Equals(TypeInfo x, TypeInfo y)
		{
			var length = _comparers.Length;
			for (var i = 0; i < length; i++)
			{
				if (_comparers[i]
					.Equals(x, y))
				{
					return true;
				}
			}

			return false;
		}

		public int GetHashCode(TypeInfo obj) => 0;
	}
}