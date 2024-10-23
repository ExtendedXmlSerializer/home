using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using JetBrains.Annotations;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Identification
{
	sealed class Identities : CacheBase<TypeInfo, IIdentity>, IIdentities
	{
		readonly IIdentityStore _source;
		readonly INames         _alias;
		readonly ITypeFormatter _formatter;
		readonly IIdentifiers   _identifiers;

		[UsedImplicitly]
		public Identities(IIdentifiers identifiers, IIdentityStore source, INames names)
			: this(source, names, TypeNameFormatter.Default, identifiers) {}

		// ReSharper disable once TooManyDependencies
		public Identities(IIdentityStore source, INames alias, ITypeFormatter formatter, IIdentifiers identifiers)
		{
			_source      = source;
			_alias       = alias;
			_formatter   = formatter;
			_identifiers = identifiers;
		}

		protected override IIdentity Create(TypeInfo parameter)
			=> _source.Get(_alias.Get(parameter) ?? _formatter.Get(parameter), _identifiers.Get(parameter));
	}
}