using System.Collections.Generic;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.Conversion.Members
{
	class MemberedActivator : DecoratedActivator
	{
		readonly IDictionary<string, IMember> _members;

		public MemberedActivator(IActivator activator, IDictionary<string, IMember> members) : base(activator)
		{
			_members = members;
		}

		public override object Get(IReader parameter)
		{
			var result = base.Get(parameter);
			var members = parameter.Members();
			while (members.MoveNext())
			{
				var member = _members.TryGet(parameter.DisplayName);
				member?.Assign(result, ((IActivator) member).Get(parameter));
			}

			return result;
		}
	}
}