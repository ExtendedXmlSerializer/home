using ExtendedXmlSerializer.Core.Sources;
using Sprache;

namespace ExtendedXmlSerializer.Core.Parsing
{
	interface IParsing<out T> : IParameterizedSource<IInput, IResult<T>> {}
}