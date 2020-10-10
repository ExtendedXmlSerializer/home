using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class ParameterizedTypeMembers : ITypeMembers
	{
		readonly ITypeMembers                 _previous;
		readonly IContainsCustomSerialization _custom;
		readonly IParameterizedMembers        _members;
		readonly Func<IMember, string>        _group;

		public ParameterizedTypeMembers(ITypeMembers previous, IContainsCustomSerialization custom,
		                                IParameterizedMembers members)
			: this(previous, custom, members, IdentityFormatter.Default.Get) {}

		// ReSharper disable once TooManyDependencies
		public ParameterizedTypeMembers(ITypeMembers previous, IContainsCustomSerialization custom,
		                                IParameterizedMembers members, Func<IMember, string> group)
		{
			_previous = previous;
			_custom   = custom;
			_members  = members;
			_group    = @group;
		}

		public ImmutableArray<IMember> Get(TypeInfo parameter)
			=> _custom.IsSatisfiedBy(parameter) ? ImmutableArray<IMember>.Empty : Implementation(parameter);

		ImmutableArray<IMember> Implementation(TypeInfo parameter)
		{
			var members = _members.Get(parameter);
			var typed   = _previous.Get(parameter);
			var items   = members?.AddRange(typed) ?? typed;

			var result = items.GroupBy(_group)
			                  .Select(x => x.OfType<ParameterizedMember>()
			                                .FirstOrDefault() ?? x.First())
			                  .ToImmutableArray();
			return result;
		}
	}
}