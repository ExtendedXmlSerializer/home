using ExtendedXmlSerializer.Core.Sources;
using Sprache;

namespace ExtendedXmlSerializer.Core.Parsing
{
	sealed class Inputs : ReferenceCache<string, IInput>
	{
		public static Inputs Default { get; } = new Inputs();

		Inputs() : base(x => new Input(x)) {}
	}
}