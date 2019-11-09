using ExtendedXmlSerializer.ContentModel.Conversion;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	struct MemberParts
	{
		public MemberParts(TypeParts type, string memberName)
		{
			Type       = type;
			MemberName = memberName;
		}

		public TypeParts Type { get; }
		public string MemberName { get; }
	}
}