using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Immutable;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	/// <summary>
	/// For extension authors, enables immutable arrays on a container.  This is a default extension used by
	/// <see cref="DefaultExtensions"/>.
	/// </summary>
	/// <seealso cref="DefaultExtensions"/>
	public sealed class ImmutableArrayExtension : ISerializerExtension
	{
		/// <summary>
		/// The instance.
		/// </summary>
		public static ImmutableArrayExtension Default { get; } = new ImmutableArrayExtension();

		ImmutableArrayExtension() : this(new IsAssignableGenericSpecification(typeof(ImmutableArray<>))) {}

		readonly ISpecification<TypeInfo> _specification;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="specification">The specification used to determine when to assign a serializer to a given type.</param>
		public ImmutableArrayExtension(ISpecification<TypeInfo> specification) => _specification = specification;

		/// <inheritdoc />
		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.DecorateContentsWith<ImmutableArrays>()
			            .When(_specification)
			            .Decorate<IGenericTypes, ImmutableArrayAwareGenericTypes>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}