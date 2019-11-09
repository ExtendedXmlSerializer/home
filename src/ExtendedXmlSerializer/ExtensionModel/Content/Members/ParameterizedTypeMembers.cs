using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Members;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class ParameterizedTypeMembers : ITypeMembers
	{
		readonly ITypeMembers          _typed;
		readonly IParameterizedMembers _members;

		public ParameterizedTypeMembers(ITypeMembers typed, IParameterizedMembers members)
		{
			_typed   = typed;
			_members = members;
		}

		public ImmutableArray<IMember> Get(TypeInfo parameter)
		{
			var members = _members.Get(parameter);
			var typed   = _typed.Get(parameter);
			var items   = members?.AddRange(typed) ?? typed;

			var result = items.GroupBy(IdentityFormatter.Default.Get)
			                  .Select(x => x.OfType<ParameterizedMember>()
			                                .FirstOrDefault() ?? x.First())
			                  .ToImmutableArray();
			return result;
		}
	}
}