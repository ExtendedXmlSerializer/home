namespace ExtendedXmlSerialization.Conversion.Members
{
	class MemberedActivator : DecoratedActivator
	{
		readonly IMembers _members;

		public MemberedActivator(IActivator activator, IMembers members) : base(activator)
		{
			_members = members;
		}

		public override object Get(IReader parameter)
		{
			var result = base.Get(parameter);
			var members = parameter.Members();
			while (members.MoveNext())
			{
				var member = _members.Get(parameter.DisplayName);
				member?.Assign(result, ((IConverter) member).Get(parameter));
			}

			return result;
		}
	}
}