using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.ContentModel.Content
{
	public interface IContainer : ISerializer, ISource<ISerializer> {}
}