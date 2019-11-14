using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using JetBrains.Annotations;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	[UsedImplicitly]
	sealed class TypeIdentities : DecoratedSource<IIdentity, TypeInfo>, ITypeIdentities
	{
		public TypeIdentities(ITypeIdentification identification) : base(identification) {}
	}
}