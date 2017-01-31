using System.Collections.Immutable;

namespace ExtendedXmlSerialization.Conversion.Names
{
	public static class Defaults
	{
		public static ImmutableArray<IName> Names { get; } = new KnownNames().ToImmutableArray();
	}

	/*public interface INames : ISelector<TypeInfo, IName> {}*/

	/*class Names : Selector<TypeInfo, IName>, INames
	{
		public Names(ImmutableArray<IName> names) : base(new KnownNamesOption(names), /*new EnumerableNameOption(locator),#1# NameOption.Default) {}

	}*/
}