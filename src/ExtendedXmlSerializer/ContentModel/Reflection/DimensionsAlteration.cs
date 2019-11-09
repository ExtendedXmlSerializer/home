using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class DimensionsAlteration : IAlteration<TypeInfo>
	{
		readonly ImmutableArray<int> _dimensions;

		public DimensionsAlteration(ImmutableArray<int> dimensions)
		{
			_dimensions = dimensions;
		}

		public TypeInfo Get(TypeInfo parameter)
		{
			var result = parameter;
			foreach (var dimension in _dimensions)
			{
				var type = dimension == 1 ? result.MakeArrayType() : result.MakeArrayType(dimension);
				result = type.GetTypeInfo();
			}

			return result;
		}
	}
}