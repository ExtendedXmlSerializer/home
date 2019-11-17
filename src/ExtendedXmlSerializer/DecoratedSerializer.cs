using ExtendedXmlSerializer.ExtensionModel.Xml;

namespace ExtendedXmlSerializer
{
	/// <summary>
	/// Convenience class to decorate an existing serializer, usually used for construction.
	/// </summary>
	public class DecoratedSerializer : ISerializer
	{
		readonly ISerializer _serializer;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="serializer">The existing serializer to decorate.</param>
		public DecoratedSerializer(ISerializer serializer) => _serializer = serializer;

		/// <inheritdoc />
		public object Deserialize(System.Xml.XmlReader reader) => _serializer.Deserialize(reader);

		/// <inheritdoc />
		public void Serialize(System.Xml.XmlWriter writer, object instance)
		{
			_serializer.Serialize(writer, instance);
		}
	}
}