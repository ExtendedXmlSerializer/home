using System.Collections;
using System.Collections.Generic;
using ExtendedXmlSerialization.ContentModel.Collections;
using ExtendedXmlSerialization.ContentModel.Members;

namespace ExtendedXmlSerialization.ContentModel.Content
{
	class ContentOptions : IContentOptions
	{
		readonly IContainers _owner;
		readonly IMembers _members;
		readonly IMemberOption _variable;
		readonly ISerializer _runtime;
		readonly IActivation _activation;
		readonly IMemberSerialization _memberSerialization;

		public ContentOptions(
			IActivation activation,
			IContainers owner,
			IMemberSerialization memberSerialization,
			IMembers members,
			IMemberOption variable, ISerializer runtime)
		{
			_owner = owner;
			_memberSerialization = memberSerialization;
			_members = members;
			_variable = variable;
			_runtime = runtime;
			_activation = activation;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public IEnumerator<IContentOption> GetEnumerator()
		{
			yield return new ArrayContentOption(_activation, _owner);
			var entries = new DictionaryEntries(_activation, _memberSerialization, _variable);
			yield return new DictionaryContentOption(_activation, _members, entries);
			yield return new CollectionContentOption(_activation, _members, _owner);
			yield return new MemberedContentOption(_activation, _members);
			yield return new RuntimeContentOption(_runtime);
		}
	}
}