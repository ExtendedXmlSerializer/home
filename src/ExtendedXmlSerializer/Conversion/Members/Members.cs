using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.Conversion.Members
{
	public class Members : IMembers
	{
		readonly ImmutableArray<IMember> _members;
		readonly IDictionary<string, IMember> _lookup;

		/*public Members(TypeInfo classification, params IMember[] members) : this(classification, members.AsEnumerable()) {}*/

		public Members(IEnumerable<IMember> members) : this(members.ToImmutableArray()) {}

		public Members(ImmutableArray<IMember> members) : this(members, members.ToDictionary(x => x.DisplayName)) {}

		public Members(ImmutableArray<IMember> members, IDictionary<string, IMember> lookup)
		{
			_members = members;
			_lookup = lookup;
		}

		public IMember Get(string parameter) => _lookup.TryGet(parameter);

		public IEnumerator<IMember> GetEnumerator()
		{
			foreach (var element in _members)
			{
				yield return element;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}