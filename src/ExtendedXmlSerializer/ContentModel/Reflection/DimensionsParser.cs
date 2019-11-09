using ExtendedXmlSerializer.Core.Parsing;
using Sprache;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class DimensionsParser : ItemsParser<int>
	{
		public static DimensionsParser Default { get; } = new DimensionsParser();

		DimensionsParser() : base(Parse.Number.Select(int.Parse)) {}
	}
}