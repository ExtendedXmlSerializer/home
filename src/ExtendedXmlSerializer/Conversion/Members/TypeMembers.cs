using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion.Members
{
	public class TypeMembers : ITypeMembers
	{
		readonly IMemberSelector _selector;
		readonly ISpecification<PropertyInfo> _property;
		readonly ISpecification<FieldInfo> _field;

		protected TypeMembers(IMemberSelector selector, ISpecification<PropertyInfo> property, ISpecification<FieldInfo> field)
		{
			_selector = selector;
			_property = property;
			_field = field;
		}

		public IEnumerable<IMember> Get(TypeInfo parameter) => Yield(parameter).OrderBy(x => x.Sort).Select(x => x.Member);

		IEnumerable<Sorting> Yield(TypeInfo parameter)
		{
			foreach (var property in parameter.GetProperties())
			{
				if (_property.IsSatisfiedBy(property))
				{
					var sorting = Create(property, property.PropertyType, property.CanWrite);
					if (sorting != null)
					{
						yield return sorting.Value;
					}
				}
			}

			foreach (var field in parameter.GetFields())
			{
				if (_field.IsSatisfiedBy(field))
				{
					var sorting = Create(field, field.FieldType, !field.IsInitOnly);
					if (sorting != null)
					{
						yield return sorting.Value;
					}
				}
			}
		}

		Sorting? Create(MemberInfo metadata, Type memberType, bool assignable)
		{
			var sort = new Sort(metadata.GetCustomAttribute<XmlElementAttribute>(false)?.Order, metadata.MetadataToken);
			var information = new MemberInformation(metadata, memberType.GetTypeInfo().AccountForNullable(), assignable);
			var element = _selector.Get(information);
			var result = element != null ? (Sorting?) new Sorting(element, sort) : null;
			return result;
		}

		struct Sorting
		{
			public Sorting(IMember member, Sort sort)
			{
				Member = member;
				Sort = sort;
			}

			public IMember Member { get; }
			public Sort Sort { get; }
		}
	}
}