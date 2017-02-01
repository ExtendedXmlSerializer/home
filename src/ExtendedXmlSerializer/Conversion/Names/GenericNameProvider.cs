using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Conversion.Xml;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Names
{
	public class GenericNameProvider : NameProviderBase<IGenericName>
	{
		readonly Func<TypeInfo, IName> _names;

		public GenericNameProvider(INames names) : this(names.Get, TypeAliasProvider.Default, TypeFormatter.Default) {}

		public GenericNameProvider(Func<TypeInfo, IName> names, IAliasProvider alias, ITypeFormatter formatter)
			: base(alias, formatter)
		{
			_names = names;
		}

		public override IGenericName Create(string displayName, TypeInfo classification)
			=>
				new GenericName(displayName, classification,
				                classification.GetGenericArguments()
				                              .Select(x => x.GetTypeInfo())
				                              .Select(_names)
				                              .ToImmutableArray());
	}
}