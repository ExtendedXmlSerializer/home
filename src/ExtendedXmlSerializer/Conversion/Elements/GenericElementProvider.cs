using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Conversion.Xml;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Elements
{
	public class GenericElementProvider : ElementProviderBase
	{
		readonly Func<TypeInfo, IElement> _names;

		public GenericElementProvider(IElements elements) : this(elements.Get, TypeAliasProvider.Default, TypeFormatter.Default) {}

		public GenericElementProvider(Func<TypeInfo, IElement> names, IAliasProvider alias, ITypeFormatter formatter)
			: base(alias, formatter)
		{
			_names = names;
		}

		public override IElement Create(string displayName, TypeInfo classification)
			=>
				new GenericElement(displayName, classification,
				                classification.GetGenericArguments()
				                              .Select(x => x.GetTypeInfo())
				                              .Select(_names)
				                              .ToImmutableArray());
	}
}