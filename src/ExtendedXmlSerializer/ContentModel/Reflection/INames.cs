using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	interface INames : INames<TypeInfo> {}

	public interface INames<in T> : IParameterizedSource<T, string> where T : MemberInfo {}
}