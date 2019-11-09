using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	interface ITypePartResolver : IParameterizedSource<TypeInfo, TypeParts> {}
}