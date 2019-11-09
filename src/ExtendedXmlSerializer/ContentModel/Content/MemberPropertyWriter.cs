using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class MemberPropertyWriter : IWriter
	{
		readonly IWriter<object> _start;
		readonly IWriter<bool>   _member;

		public MemberPropertyWriter(IWriter<object> start) : this(start, MemberProperty.Default) {}

		public MemberPropertyWriter(IWriter<object> start, IWriter<bool> member)
		{
			_start  = start;
			_member = member;
		}

		public void Write(IFormatWriter writer, object instance)
		{
			_start.Write(writer, instance);
			_member.Write(writer, true);
		}
	}
}