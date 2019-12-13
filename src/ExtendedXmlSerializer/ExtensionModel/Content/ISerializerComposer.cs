using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	/// <summary>
	/// Component used to alter content serializers created by the root serializer.
	/// </summary>
	public interface ISerializerComposer : IAlteration<ContentModel.ISerializer> {}

	/// <summary>
	/// Component used to alter content serializers created by the root serializer.
	/// </summary>
	public interface ISerializerComposer<T> : IAlteration<ContentModel.ISerializer<T>> {}
}