using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberWriter : IWriter
	{
		readonly IMemberAccess   _access;
		readonly IWriter<object> _writer;

		public MemberWriter(IMemberAccess access, IWriter<object> writer)
		{
			_access = access;
			_writer = writer;
		}

		public void Write(IFormatWriter writer, object instance)
		{
			if (_access.Instance.IsSatisfiedBy(instance))
			{
				var member = _access.Get(instance);
				if (_access.IsSatisfiedBy(member))
				{
					_writer.Write(writer, member);
				}
			}
		}
	}
}