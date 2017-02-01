using System.Reflection;

namespace ExtendedXmlSerialization.Conversion.Elements
{
	public abstract class AliasProviderBase<T> : IAliasProvider where T : MemberInfo
	{
		public string Get(MemberInfo parameter) => GetItem((T) parameter);

		protected abstract string GetItem(T parameter);
	}
}