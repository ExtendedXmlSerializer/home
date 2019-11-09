namespace ExtendedXmlSerializer
{
	public class DecoratedSerializer<TRead, TWrite> : ISerializer<TRead, TWrite>
	{
		readonly ISerializer<TRead, TWrite> _serializer;

		public DecoratedSerializer(ISerializer<TRead, TWrite> serializer) => _serializer = serializer;

		public object Deserialize(TRead reader) => _serializer.Deserialize(reader);

		public void Serialize(TWrite writer, object instance)
		{
			_serializer.Serialize(writer, instance);
		}
	}
}