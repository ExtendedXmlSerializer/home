using System;

namespace ExtendedXmlSerialization
{
    /// <summary>
    /// Interface Extended Xml Serializer
    /// </summary>
    public interface IExtendedXmlSerializer
    {
        /// <summary>
        /// Serializes the specified <see cref="T:System.Object" /> and returns xml document in string
        /// </summary>
        /// <param name="o">The <see cref="T:System.Object" /> to serialize. </param>
        /// <returns>xml document in string</returns>
        string Serialize(object o);

        /// <summary>
        /// Deserializes the XML document
        /// </summary>
        /// <param name="xml">The XML document</param>
        /// <param name="type">The type of returned object</param>
        /// <returns>deserialized object</returns>
        object Deserialize(string xml, Type type);

        /// <summary>
        /// Deserializes the XML document
        /// </summary>
        /// <typeparam name="T">The type of returned object</typeparam>
        /// <param name="xml">The XML document</param>
        /// <returns>deserialized object</returns>
        T Deserialize<T>(string xml);

        /// <summary>
        /// Gets or sets <see cref="ISerializationToolsFactory"/>
        /// </summary>
        ISerializationToolsFactory SerializationToolsFactory { get; set; }
    }
}