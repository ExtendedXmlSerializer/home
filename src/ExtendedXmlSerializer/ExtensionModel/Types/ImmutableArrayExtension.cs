using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Immutable;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	public sealed class ImmutableArrayExtension : ISerializerExtension
	{
		public static ImmutableArrayExtension Default { get; } = new ImmutableArrayExtension();

		ImmutableArrayExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter
			   .DecorateContent<ImmutableArrays>(new IsAssignableGenericSpecification(typeof(ImmutableArray<>)))
			   .Decorate<IGenericTypes, ImmutableArrayAwareGenericTypes>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}