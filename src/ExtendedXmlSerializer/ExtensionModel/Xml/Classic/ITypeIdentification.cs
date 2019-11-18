using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Sources;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	interface ITypeIdentification
		: IParameterizedSource<IIdentity, TypeInfo>, IParameterizedSource<TypeInfo, IIdentity> {}
}