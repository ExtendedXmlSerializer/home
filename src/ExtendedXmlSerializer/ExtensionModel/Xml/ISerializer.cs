namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	/// <summary>
	/// A baseline XML-based serializer.
	/// </summary>
	public interface ISerializer
	{
		/// <summary>
		/// Performs a read operation over the provided document source and resolves an object instance from it.
		/// </summary>
		/// <param name="reader">The reader that represents the document source.</param>
		/// <returns>The instance that is represented by the document.</returns>
		object Deserialize(System.Xml.XmlReader reader);

		/// <summary>
		/// Performance a write operation with the provided object instance, and saves it to the destination, represented by
		/// the provided writer.
		/// </summary>
		/// <param name="writer">The destination writer.</param>
		/// <param name="instance">The source instance.</param>
		void Serialize(System.Xml.XmlWriter writer, object instance);
	}
}