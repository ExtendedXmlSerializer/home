using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.Conversion
{
	public class Members : IMembers
	{
		readonly ImmutableArray<IMemberContext> _members;
		readonly IDictionary<string, IMemberContext> _lookup;

		/*public Members(TypeInfo classification, params IMember[] members) : this(classification, members.AsEnumerable()) {}*/

		public Members(IEnumerable<IMemberContext> members) : this(members.ToImmutableArray()) {}

		public Members(ImmutableArray<IMemberContext> members) : this(members, members.ToDictionary(x => x.DisplayName)) {}

		public Members(ImmutableArray<IMemberContext> members, IDictionary<string, IMemberContext> lookup)
		{
			_members = members;
			_lookup = lookup;
		}

		public IMemberContext Get(string parameter) => _lookup.TryGet(parameter);

		public IEnumerator<IMemberContext> GetEnumerator()
		{
			foreach (var element in _members)
			{
				yield return element;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}