using ExtendedXmlSerializer.Core.Sources;
using System;

namespace ExtendedXmlSerializer.ContentModel.Identification
{
	sealed class IdentityStore : Cache<string, Func<string, IIdentity>>, IIdentityStore
	{
		public IdentityStore() : base(i => new Names(i).Get) {}

		public IIdentity Get(string name, string identifier) => Get(identifier).Invoke(name);

		sealed class Names : CacheBase<string, IIdentity>
		{
			readonly string _identifier;

			public Names(string identifier) => _identifier = identifier;

			protected override IIdentity Create(string parameter) => new Identity(parameter, _identifier);
		}
	}
}