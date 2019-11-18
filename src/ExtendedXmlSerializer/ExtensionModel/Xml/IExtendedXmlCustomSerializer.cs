using System.Xml.Linq;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	/// <summary>
	/// A strongly-typed v1 custom serializer.
	/// </summary>
	/// <typeparam name="T">The type to serialize.</typeparam>
	public interface IExtendedXmlCustomSerializer<T>
	{
		/// <summary>
		/// Deserializes the document subtree from the provided element into a new instance.
		/// </summary>
		/// <param name="xElement">The element subtree.</param>
		/// <returns>An instance from the provided element subtree.</returns>
		T Deserialize(XElement xElement);

		/// <summary>
		/// Serializes an instance.
		/// </summary>
		/// <param name="xmlWriter">The writer holding the destination document.</param>
		/// <param name="obj">The instance to serialize.</param>
		void Serializer(System.Xml.XmlWriter xmlWriter, T obj);
	}

	/// <summary>
	/// The v1 custom serializer.
	/// </summary>
	public interface IExtendedXmlCustomSerializer
	{
		/// <summary>
		/// Deserializes the document subtree from the provided element into a new instance.
		/// </summary>
		/// <param name="xElement">The element subtree.</param>
		/// <returns>An instance from the provided element subtree.</returns>
		object Deserialize(XElement xElement);

		/// <summary>
		/// Serializes an instance.
		/// </summary>
		/// <param name="xmlWriter">The writer holding the destination document.</param>
		/// <param name="instance">The instance to serialize.</param>
		void Serializer(System.Xml.XmlWriter xmlWriter, object instance);
	}
}