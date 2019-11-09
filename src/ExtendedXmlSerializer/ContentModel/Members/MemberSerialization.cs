using System.Collections.Immutable;
using System.Linq;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberSerialization : TableSource<string, IMemberSerializer>, IMemberSerialization
	{
		readonly IRuntimeMemberList                _runtime;
		readonly ImmutableArray<IMemberSerializer> _all;

		public MemberSerialization(IRuntimeMemberList runtime, ImmutableArray<IMemberSerializer> all)
			: base(all.ToDictionary(x => IdentityFormatter.Default.Get(x.Profile)))
		{
			_runtime = runtime;
			_all     = all;
		}

		public ImmutableArray<IMemberSerializer> Get(object parameter) => _runtime.Get(parameter);

		public ImmutableArray<IMemberSerializer> Get() => _all;
	}
}