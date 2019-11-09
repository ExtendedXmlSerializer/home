using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	interface ISerializers : IParameterizedSource<TypeInfo, ISerializer> {}
}