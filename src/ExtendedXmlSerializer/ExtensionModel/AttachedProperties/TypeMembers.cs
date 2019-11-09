using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.AttachedProperties
{
	sealed class TypeMembers : ITypeMembers
	{
		readonly IIdentities               _identities;
		readonly ITypeMembers              _typeMembers;
		readonly IMembers                  _members;
		readonly ImmutableArray<IProperty> _properties;

		// ReSharper disable once TooManyDependencies
		public TypeMembers(IIdentities identities, ITypeMembers typeMembers, IMembers members,
		                   ImmutableArray<IProperty> properties)
		{
			_identities  = identities;
			_typeMembers = typeMembers;
			_members     = members;
			_properties  = properties;
		}

		public ImmutableArray<IMember> Get(TypeInfo parameter)
			=> _typeMembers.Get(parameter)
			               .AddRange(Yield(parameter)
				                         .OrderBy(x => x.Order));

		IEnumerable<IMember> Yield(TypeInfo parameter)
		{
			foreach (var property in _properties)
			{
				if (property.IsSatisfiedBy(parameter))
				{
					var propertyInfo = property.Metadata;
					var identity     = _identities.Get(propertyInfo.DeclaringType);
					yield return new AttachedMember(identity, _members.Get(propertyInfo), property);
				}
			}
		}
	}
}