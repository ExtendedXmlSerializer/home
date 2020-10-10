using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	interface IDictionaryPairTypesLocator : IParameterizedSource<TypeInfo, DictionaryPairTypes?> {}
}