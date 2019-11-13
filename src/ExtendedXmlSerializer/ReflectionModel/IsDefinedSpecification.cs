using System;
using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class IsDefinedSpecification<T> : ISpecification<MemberInfo>
	{
		readonly static Type Type = Support<T>.Key;

		public static IsDefinedSpecification<T> Default { get; } = new IsDefinedSpecification<T>();

		IsDefinedSpecification() : this(true) {}

		readonly bool _inherit;

		public IsDefinedSpecification(bool inherit) => _inherit = inherit;

		public bool IsSatisfiedBy(MemberInfo parameter) => parameter.IsDefined(Type, _inherit);
	}
}