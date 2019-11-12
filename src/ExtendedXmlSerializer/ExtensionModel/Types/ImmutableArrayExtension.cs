using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Immutable;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	public sealed class ImmutableArrayExtension : ISerializerExtension
	{
		public static ImmutableArrayExtension Default { get; } = new ImmutableArrayExtension();

		ImmutableArrayExtension() : this(new IsAssignableGenericSpecification(typeof(ImmutableArray<>))) {}

		readonly ISpecification<TypeInfo> _specification;

		public ImmutableArrayExtension(ISpecification<TypeInfo> specification) => _specification = specification;

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.DecorateContentsWith<ImmutableArrays>()
			            .When(_specification)
			            .Decorate<IGenericTypes, ImmutableArrayAwareGenericTypes>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}