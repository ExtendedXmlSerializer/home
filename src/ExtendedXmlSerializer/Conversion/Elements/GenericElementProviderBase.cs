using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Conversion.Xml;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Elements
{
	abstract class GenericElementProviderBase : ElementProviderBase
	{
		readonly Func<TypeInfo, IElement> _names;

		protected GenericElementProviderBase(Func<TypeInfo, IElement> names, IAliasProvider alias)
			: this(names, alias, TypeFormatter.Default) {}

		protected GenericElementProviderBase(Func<TypeInfo, IElement> names, IAliasProvider alias, ITypeFormatter formatter)
			: base(alias, formatter)
		{
			_names = names;
		}

		public override IElement Create(string displayName, TypeInfo classification)
			=> Create(displayName, classification,
			          classification.GetGenericArguments()
			                        .Select(x => x.GetTypeInfo())
			                        .Select(_names)
			                        .ToImmutableArray());

		protected abstract IElement Create(string displayName, TypeInfo classification,
		                                   ImmutableArray<IElement> arguments);
	}
}