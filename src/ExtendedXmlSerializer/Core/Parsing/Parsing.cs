using ExtendedXmlSerializer.Core.Sources;
using Sprache;

namespace ExtendedXmlSerializer.Core.Parsing
{
	class Parsing<T> : DelegatedSource<IInput, IResult<T>>, IParsing<T>
	{
		public static implicit operator Parser<T>(Parsing<T> instance) => instance.Get;

		public Parsing(Parser<T> parser) : base(parser.Invoke) {}
	}
}