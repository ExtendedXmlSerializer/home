using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerialization.Conversion.Model.Names;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion
{
	class Names : INames
	{
		readonly IParameterizedSource<TypeInfo, IName> _selector;
		public Names() : this(Defaults.Names) {}

		public Names(ImmutableArray<IName> names)
		{
			_selector = new Selector<TypeInfo, IName>(new KnownNamesOption(names), new GenericNameOption(this),
			                                          NameOption.Default);
		}

		public IName Get(TypeInfo parameter) => _selector.Get(parameter);
	}
}