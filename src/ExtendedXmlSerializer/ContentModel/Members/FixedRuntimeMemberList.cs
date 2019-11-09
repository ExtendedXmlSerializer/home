using System.Collections.Immutable;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class FixedRuntimeMemberList : IRuntimeMemberList
	{
		readonly ImmutableArray<IMemberSerializer> _serializers;

		public FixedRuntimeMemberList(ImmutableArray<IMemberSerializer> serializers)
		{
			_serializers = serializers;
		}

		public ImmutableArray<IMemberSerializer> Get(object parameter) => _serializers;
	}
}