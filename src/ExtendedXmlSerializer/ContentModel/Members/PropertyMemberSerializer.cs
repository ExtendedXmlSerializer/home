using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class PropertyMemberSerializer : IMemberSerializer
	{
		readonly IMemberSerializer _serializer;

		public PropertyMemberSerializer(IMemberSerializer serializer) => _serializer = serializer;

		public object Get(IFormatReader parameter) => _serializer.Get(parameter);

		public void Write(IFormatWriter writer, object instance) => _serializer.Write(writer, instance);

		public IMember Profile => _serializer.Profile;

		public IMemberAccess Access => _serializer.Access;
	}
}