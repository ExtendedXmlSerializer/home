using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Sprache;

namespace ExtendedXmlSerializer.Core.Parsing
{
	interface IParsing<out T> : IParameterizedSource<IInput, IResult<T>> {}
}