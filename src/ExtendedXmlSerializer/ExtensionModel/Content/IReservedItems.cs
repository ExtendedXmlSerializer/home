using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.References;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	interface IReservedItems : IParameterizedSource<IFormatWriter, ITrackedLists> {}
}