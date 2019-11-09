using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ExtensionModel.Types;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class ParameterizedMembers : IParameterizedMembers
	{
		readonly IQueriedConstructors _constructors;
		readonly IConstructorMembers  _members;

		public ParameterizedMembers(IQueriedConstructors constructors, IConstructorMembers members)
		{
			_constructors = constructors;
			_members      = members;
		}

		public ImmutableArray<IMember>? Get(TypeInfo parameter)
		{
			var constructor = _constructors.Get(parameter);
			var result      = constructor != null ? _members.Get(constructor) : null;
			return result;
		}
	}
}