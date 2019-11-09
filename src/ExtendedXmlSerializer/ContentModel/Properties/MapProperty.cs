using System.Collections.Immutable;
using System.Linq;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Parsing;

namespace ExtendedXmlSerializer.ContentModel.Properties
{
	sealed class MapProperty : DelegatedProperty<ImmutableArray<int>>
	{
		public static MapProperty Default { get; } = new MapProperty();

		MapProperty() : base(DimensionsParser.Default.Get, x => string.Join(",", x.ToArray()),
		                     new FrameworkIdentity("map")) {}
	}
}