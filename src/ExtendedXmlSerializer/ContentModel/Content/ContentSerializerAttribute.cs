using System;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	/// <summary>
	/// Used to configure a serializer for a type member.
	/// </summary>
	/// <seealso href="https://github.com/ExtendedXmlSerializer/ExtendedXmlSerializer/issues/150"/>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class ContentSerializerAttribute : Attribute
	{
		/// <summary>
		/// Creates an instance.
		/// </summary>
		/// <param name="serializerType">The serializer type.  Must implement <see cref="ISerializer{T}"/> or
		/// <see cref="ISerializer"/></param>
		public ContentSerializerAttribute(Type serializerType) => SerializerType = serializerType;

		/// <summary>
		/// This object's serializer type.
		/// </summary>
		public Type SerializerType { get; }
	}
}