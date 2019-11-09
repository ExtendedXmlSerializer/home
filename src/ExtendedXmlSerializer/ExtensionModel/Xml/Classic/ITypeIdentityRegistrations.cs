using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	interface ITypeIdentityRegistrations
		: IParameterizedSource<IEnumerable<TypeInfo>, IEnumerable<KeyValuePair<TypeInfo, IIdentity>>> {}
}