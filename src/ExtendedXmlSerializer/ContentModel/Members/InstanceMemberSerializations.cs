using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class InstanceMemberSerializations : IInstanceMemberSerializations
	{
		readonly ISpecification<TypeInfo> _specification;
		readonly IMemberSerializations    _serializations;

		public InstanceMemberSerializations(IMemberSerializations serializations)
			: this(VariableTypeSpecification.Default, serializations) {}

		public InstanceMemberSerializations(ISpecification<TypeInfo> specification,
		                                    IMemberSerializations serializations)
		{
			_specification  = specification;
			_serializations = serializations;
		}

		public IInstanceMemberSerialization Get(TypeInfo parameter)
			=> _specification.IsSatisfiedBy(parameter)
				   ? (IInstanceMemberSerialization)new InstanceMemberSerialization(parameter, _serializations)
				   : new FixedInstanceMemberSerialization(_serializations.Get(parameter));
	}
}