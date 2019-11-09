using ExtendedXmlSerializer.ContentModel.Identification;

namespace ExtendedXmlSerializer.ContentModel.Properties
{
	interface IProperty<T> : ISerializer<T>, IIdentity {}
}