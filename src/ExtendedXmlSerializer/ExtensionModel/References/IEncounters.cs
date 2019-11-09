using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	interface IEncounters : ISpecification<object>, IParameterizedSource<object, Identifier?> {}
}