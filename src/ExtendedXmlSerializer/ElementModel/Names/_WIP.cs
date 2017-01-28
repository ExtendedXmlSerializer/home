using System.Collections.Immutable;
using System.Reflection;
using System.Xml.Serialization;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.ElementModel.Names
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

	public interface IAliasProvider : IParameterizedSource<MemberInfo, string> {}

	public abstract class AliasProviderBase<T> : IAliasProvider where T : MemberInfo
	{
		public string Get(MemberInfo parameter) => GetItem((T) parameter);

		protected abstract string GetItem(T parameter);
	}

	class TypeAliasProvider : AliasProviderBase<TypeInfo>
	{
		public static TypeAliasProvider Default { get; } = new TypeAliasProvider();
		TypeAliasProvider() {}

		protected override string GetItem(TypeInfo parameter) =>
			parameter.GetCustomAttribute<XmlRootAttribute>()?.ElementName;
	}
}