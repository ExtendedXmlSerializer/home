using ExtendedXmlSerializer.Core;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class StaticReferenceCheckingExtension : ISerializerExtension
	{
		public static StaticReferenceCheckingExtension Default { get; } = new StaticReferenceCheckingExtension();

		StaticReferenceCheckingExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterInstance<IStaticReferenceSpecification>(Specification.Instance);

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class Specification : IStaticReferenceSpecification
		{
			public static Specification Instance { get; } = new Specification();

			Specification() {}

			public bool IsSatisfiedBy(TypeInfo parameter) => false;
		}
	}
}