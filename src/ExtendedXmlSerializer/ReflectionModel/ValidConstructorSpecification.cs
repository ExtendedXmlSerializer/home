using System;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class ValidConstructorSpecification : IValidConstructorSpecification
	{
		public static ValidConstructorSpecification Default { get; } = new ValidConstructorSpecification();

		ValidConstructorSpecification() {}

		public bool IsSatisfiedBy(ConstructorInfo parameter)
		{
			var parameters = parameter.GetParameters();
			var result = parameters.Length == 0 ||
			             parameters.All(x => x.IsOptional || x.IsDefined(typeof(ParamArrayAttribute)));
			return result;
		}
	}
}