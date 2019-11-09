using System;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class ContentSerializerAttribute : Attribute
	{
		public ContentSerializerAttribute(Type serializerType) => SerializerType = serializerType;

		public Type SerializerType { get; }
	}
}