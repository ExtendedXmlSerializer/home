using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	interface IConverters : IParameterizedSource<TypeInfo, IConverter> {}
}