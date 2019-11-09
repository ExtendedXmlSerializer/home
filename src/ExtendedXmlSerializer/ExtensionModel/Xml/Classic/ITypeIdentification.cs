using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	public interface ITypeIdentification
		: IParameterizedSource<IIdentity, TypeInfo>, IParameterizedSource<TypeInfo, IIdentity> {}
}