using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.Conversion.Members
{
	public class Members : IMembers
	{
		readonly ImmutableArray<IMemberConverter> _members;
		readonly IDictionary<string, IMemberConverter> _lookup;

		/*public Members(TypeInfo classification, params IMember[] members) : this(classification, members.AsEnumerable()) {}*/

		public Members(IEnumerable<IMemberConverter> members) : this(members.ToImmutableArray()) {}

		public Members(ImmutableArray<IMemberConverter> members) : this(members, members.ToDictionary(x => x.DisplayName)) {}

		public Members(ImmutableArray<IMemberConverter> members, IDictionary<string, IMemberConverter> lookup)
		{
			_members = members;
			_lookup = lookup;
		}

		public IMemberConverter Get(string parameter) => _lookup.TryGet(parameter);

		public IEnumerator<IMemberConverter> GetEnumerator()
		{
			foreach (var element in _members)
			{
				yield return element;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}