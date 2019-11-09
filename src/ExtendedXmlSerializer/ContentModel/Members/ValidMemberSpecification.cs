using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class ValidMemberSpecification : AllSpecification<IMember>, IValidMemberSpecification
	{
		public ValidMemberSpecification(IMemberAccessors accessors) : base(
		                                                                   new DelegatedSpecification<IMember>(x =>
			                                                                                                       !x
			                                                                                                        .Metadata
			                                                                                                        .DeclaringType
			                                                                                                        .GetTypeInfo()
			                                                                                                        .IsGenericTypeDefinition),
		                                                                   accessors.IfAssigned()) {}
	}
}