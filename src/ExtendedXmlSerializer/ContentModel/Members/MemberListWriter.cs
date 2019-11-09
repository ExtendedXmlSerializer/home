using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberListWriter : IWriter
	{
		readonly IInstanceMemberSerialization _serialization;

		public MemberListWriter(IInstanceMemberSerialization serialization) => _serialization = serialization;

		public void Write(IFormatWriter writer, object instance)
		{
			if (instance != null)
			{
				var members = _serialization.Get(instance)
				                            .Get(instance);
				var length = members.Length;
				for (var i = 0; i < length; i++)
				{
					members[i]
						.Write(writer, instance);
				}
			}
			else
			{
				writer.Content(null);
			}
		}
	}
}