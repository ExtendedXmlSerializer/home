using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	interface ITypeIdentity : IParameterizedSource<TypeInfo, Key?> {}
}