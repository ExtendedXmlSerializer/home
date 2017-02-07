using System;
using System.Reflection;
using ExtendedXmlSerialization.ConverterModel.Elements;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.ConverterModel.Members
{
	class VariableTypeMember : Member, IVariableTypeMember
	{
		readonly ISpecification<Type> _specification;

		public VariableTypeMember(string displayName, TypeInfo classification, Func<object, object> getter,
		                          Action<object, object> setter, IConverter body)
			: this(displayName, getter, setter, new VariableTypedMember(displayName, classification.AsType()), body) {}

		VariableTypeMember(string displayName, Func<object, object> getter, Action<object, object> setter,
		                   IVariableTypedMember element, IConverter body)
			: base(displayName, getter, setter, element, body)
		{
			_specification = element;
		}

		public bool IsSatisfiedBy(Type parameter) => _specification.IsSatisfiedBy(parameter);
	}
}