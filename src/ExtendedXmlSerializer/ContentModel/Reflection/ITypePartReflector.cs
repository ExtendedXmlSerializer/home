using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	interface ITypePartReflector : IParameterizedSource<TypeParts, TypeInfo> {}
}