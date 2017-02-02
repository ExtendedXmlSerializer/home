using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.Elements
{
	public class Elements : IElements
	{
		readonly IParameterizedSource<TypeInfo, IElement> _selector;
		public Elements() : this(Defaults.Elements) {}

		public Elements(ImmutableArray<IElement> elements)
		{
			_selector = new Selector<TypeInfo, IElement>(new KnownElementsOption(elements), new GenericElementOption(this),
			                                          ElementOption.Default);
		}

		public IElement Get(TypeInfo parameter) => _selector.Get(parameter);
	}
}