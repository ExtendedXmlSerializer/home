using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	interface IEntity : IParameterizedSource<object, string>, IParameterizedSource<IFormatReader, object>
	{
		object Reference(IFormatReader parameter);
	}
}