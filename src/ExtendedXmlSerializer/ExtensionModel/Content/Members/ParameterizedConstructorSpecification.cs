using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class ParameterizedConstructorSpecification
		: AnySpecification<ConstructorInfo>, IValidConstructorSpecification
	{
		public ParameterizedConstructorSpecification(IValidConstructorSpecification specification,
		                                             IConstructorMembers source)
			: base(specification, source.IfAssigned()) {}
	}
}