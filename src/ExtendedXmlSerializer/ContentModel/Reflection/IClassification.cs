using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	interface IClassification : IParameterizedSource<IFormatReader, TypeInfo> {}
}