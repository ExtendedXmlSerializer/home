using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;

namespace ExtendedXmlSerializer.ContentModel.Format
{
	class MemberParsedReader : ParsedReader<MemberInfo>
	{
		public MemberParsedReader(IIdentity identity) : base(x => x, identity) {}
	}
}