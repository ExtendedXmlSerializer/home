using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberNames : Cache<MemberInfo, string>, INames
	{
		public MemberNames(IParameterizedSource<MemberInfo, string> source) : base(source.Get) {}
	}
}