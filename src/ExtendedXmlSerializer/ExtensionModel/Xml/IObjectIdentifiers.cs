using System.Collections.Immutable;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	interface IObjectIdentifiers : IParameterizedSource<object, ImmutableArray<string>> {}
}