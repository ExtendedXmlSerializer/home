using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion
{
	public class Contexts : IContexts
	{
		readonly IParameterizedSource<TypeInfo, IElementContext> _source;

		public Contexts(INames names)
		{
			_source = new Selector<TypeInfo, IElementContext>(new ContextOptions(this, names).ToArray());
		}

		public IElementContext Get(TypeInfo parameter) => _source.Get(parameter);
	}
}