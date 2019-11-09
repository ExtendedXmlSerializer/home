namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class Members : IMembers
	{
		readonly INames       _names;
		readonly IMemberOrder _order;

		public Members(INames names, IMemberOrder order)
		{
			_names = names;
			_order = order;
		}

		public IMember Get(MemberDescriptor parameter)
		{
			var metadata = parameter.Metadata;
			var order    = _order.Get(metadata);
			var name     = _names.Get(metadata) ?? metadata.Name;
			var result   = new Member(name, order, metadata, parameter.MemberType, parameter.Writable);
			return result;
		}
	}
}