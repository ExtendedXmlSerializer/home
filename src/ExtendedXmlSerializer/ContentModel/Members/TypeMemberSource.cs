using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	class TypeMemberSource : ITypeMemberSource
	{
		readonly IMetadataSpecification _specification;
		readonly IProperties            _properties;
		readonly IFields                _fields;
		readonly IMembers               _members;

		// ReSharper disable once TooManyDependencies
		public TypeMemberSource(IMetadataSpecification specification, IProperties properties, IFields fields,
		                        IMembers members)
		{
			_specification = specification;
			_properties    = properties;
			_fields        = fields;
			_members       = members;
		}

		public IEnumerable<IMember> Get(TypeInfo parameter)
		{
			var properties = _properties.Get(parameter)
			                            .ToArray();
			var length = properties.Length;
			for (var i = 0; i < length; i++)
			{
				var property = properties[i];
				if (_specification.IsSatisfiedBy(property))
				{
					yield return _members.Get(property);
				}
			}

			var fields = _fields.Get(parameter)
			                    .ToArray();
			var l = fields.Length;
			for (var i = 0; i < l; i++)
			{
				var field = fields[i];
				if (_specification.IsSatisfiedBy(field))
				{
					yield return _members.Get(field);
				}
			}
		}
	}
}