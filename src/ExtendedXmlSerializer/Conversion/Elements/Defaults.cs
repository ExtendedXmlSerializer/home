using System.Collections.Immutable;

namespace ExtendedXmlSerialization.Conversion.Elements
{
	public static class Defaults
	{
		public static ImmutableArray<IElement> Names { get; } = new KnownElements().ToImmutableArray();
	}
}