using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ExtendedXmlSerializer.ContentModel.Identification;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	sealed class TypePartsEqualityComparer : IEqualityComparer<TypeParts>
	{
		readonly static IdentityComparer<TypeParts> Comparer = IdentityComparer<TypeParts>.Default;

		public static TypePartsEqualityComparer Default { get; } = new TypePartsEqualityComparer();

		TypePartsEqualityComparer() {}

		public bool Equals(TypeParts x, TypeParts y)
		{
			var argumentsX = x.GetArguments()
			                  .GetValueOrDefault(ImmutableArray<TypeParts>.Empty);
			var argumentsY = y.GetArguments()
			                  .GetValueOrDefault(ImmutableArray<TypeParts>.Empty);

			var arguments = argumentsX.SequenceEqual(argumentsY, this);
			var identity  = Comparer.Equals(x, y);

			var dimensions = x.Dimensions.GetValueOrDefault(ImmutableArray<int>.Empty)
			                  .SequenceEqual(y.Dimensions.GetValueOrDefault(ImmutableArray<int>.Empty));

			var result = arguments && identity && dimensions;
			return result;
		}

		public int GetHashCode(TypeParts obj)
		{
			unchecked
			{
				return ((obj.Dimensions?.GetHashCode() ?? 0) * 489) ^
				       ((obj.GetArguments()
				            ?.GetHashCode() ?? 0) * 397) ^
				       Comparer.GetHashCode(obj);
			}
		}
	}
}