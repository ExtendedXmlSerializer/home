using System.Collections;
using System.Collections.Generic;
using ExtendedXmlSerialization.ContentModel.Collections;
using ExtendedXmlSerialization.ContentModel.Members;

namespace ExtendedXmlSerialization.ContentModel.Content
{
	class ContentOptions : IContentOptions
	{
		readonly ISerialization _owner;
		readonly IMembers _members;
		readonly IMemberOption _variable;
		readonly ISerializer _runtime;
		readonly IMemberSerialization _memberSerialization;

		public ContentOptions(
			ISerialization owner,
			IMemberSerialization memberSerialization,
			IMembers members,
			IMemberOption variable,
			ISerializer runtime)
		{
			_owner = owner;
			_memberSerialization = memberSerialization;
			_members = members;
			_variable = variable;
			_runtime = runtime;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public IEnumerator<IContentOption> GetEnumerator()
		{
			yield return new ArrayContentOption(_owner);
			yield return new DictionaryContentOption(_members, new DictionaryEntries(_memberSerialization, _variable));
			yield return new CollectionContentOption(_members, _owner);
			yield return new MemberedContentOption(_members);
			yield return new RuntimeContentOption(_runtime);
		}
	}
}