using ExtendedXmlSerializer.ContentModel.Conversion;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	readonly struct MemberParts
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