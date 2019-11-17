using ExtendedXmlSerializer.ContentModel.Content;

namespace ExtendedXmlSerializer.ContentModel
{
	/// <summary>
	/// A specialized <see cref="IContents"/> that is used during runtime when no serializer can be located for a provided type.
	/// </summary>
	public interface IRuntimeSerialization : IContents {}
}