using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	class PropertyMemberSpecification : ISpecification<PropertyInfo>
	{
		public static PropertyMemberSpecification Default { get; } = new PropertyMemberSpecification();

		PropertyMemberSpecification() {}

		public bool IsSatisfiedBy(PropertyInfo parameter)
		{
			var getMethod = parameter.GetGetMethod(true);
			var result = parameter.CanRead && !getMethod.IsStatic && getMethod.IsPublic &&
			             !(!parameter.GetSetMethod(true)
			                         ?.IsPublic ?? false) &&
			             parameter.GetIndexParameters()
			                      .Length <= 0;
			return result;
		}
	}
}