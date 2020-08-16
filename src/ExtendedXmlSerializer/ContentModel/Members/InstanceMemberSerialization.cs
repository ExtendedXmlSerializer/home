using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Types;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class InstanceMemberSerialization : IInstanceMemberSerialization
	{
		readonly ISpecification<TypeInfo> _specification;
		readonly IMemberSerializations    _serializations;
		readonly IMemberSerialization     _serialization;

		public InstanceMemberSerialization(TypeInfo type, IMemberSerializations serializations)
			: this(VariableTypeSpecification.Defaults.Get(type), serializations, serializations.Get(type)) {}

		public InstanceMemberSerialization(ISpecification<TypeInfo> specification, IMemberSerializations serializations,
		                                   IMemberSerialization serialization)
		{
			_specification  = specification;
			_serializations = serializations;
			_serialization  = serialization;
		}

		public IMemberSerialization Get(object parameter)
		{
			var type   = parameter is ITypeAware aware ? aware.Get() : parameter.GetType().GetTypeInfo();
			var result = _specification.IsSatisfiedBy(type) ? _serializations.Get(type) : _serialization;
			return result;
		}
	}
}