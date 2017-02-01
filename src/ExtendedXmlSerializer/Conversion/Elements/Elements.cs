using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.Elements
{
	public class Elements : IElements
	{
		readonly IParameterizedSource<TypeInfo, IElement> _selector;
		public Elements() : this(Defaults.Names) {}

		public Elements(ImmutableArray<IElement> names)
		{
			_selector = new Selector<TypeInfo, IElement>(new KnownElementsOption(names), new GenericElementOption(this),
			                                          ElementOption.Default);
		}

		public IElement Get(TypeInfo parameter) => _selector.Get(parameter);
	}
}