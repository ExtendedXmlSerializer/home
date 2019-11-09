using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.Core.Parsing
{
	public interface IParser<out T> : IParameterizedSource<string, T> {}
}