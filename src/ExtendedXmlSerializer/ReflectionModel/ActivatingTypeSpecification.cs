using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class ActivatingTypeSpecification : AnySpecification<TypeInfo>, IActivatingTypeSpecification
	{
		public static ActivatingTypeSpecification Default { get; } = new ActivatingTypeSpecification();

		ActivatingTypeSpecification() : this(ConstructorLocator.Default) {}

		public ActivatingTypeSpecification(IConstructorLocator locator)
			: base(
			       IsValueTypeSpecification.Default.Or(
			                                           ActivatedTypeSpecification.Default
			                                                                     .And(locator.IfAssigned()))) {}
	}
}