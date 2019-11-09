using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	interface ITypeIdentifications : IParameterizedSource<IEnumerable<TypeInfo>, ITypeIdentification> {}
}