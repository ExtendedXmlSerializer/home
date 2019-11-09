using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	sealed class TypeIdentities : DecoratedSource<IIdentity, TypeInfo>, ITypeIdentities
	{
		public TypeIdentities(ITypeIdentification identification) : base(identification) {}
	}
}