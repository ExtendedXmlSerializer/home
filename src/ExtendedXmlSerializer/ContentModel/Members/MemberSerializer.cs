using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberSerializer : IMemberSerializer
	{
		readonly IReader _reader;
		readonly IWriter _writer;

		// ReSharper disable once TooManyDependencies
		public MemberSerializer(IMember profile, IMemberAccess access, IReader reader, IWriter writer)
		{
			Profile = profile;
			Access  = access;
			_reader = reader;
			_writer = writer;
		}

		public IMember Profile { get; }
		public IMemberAccess Access { get; }

		public object Get(IFormatReader parameter) => _reader.Get(parameter);

		public void Write(IFormatWriter writer, object instance) => _writer.Write(writer, instance);
	}
}