using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Content;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel
{
	/// <summary>
	/// Used to establish a fluent context for decorating the <see cref="IElement"/> component when configuring the
	/// configuration container.
	/// </summary>
	/// <typeparam name="T">The decorating type of type IElement with which to establish the context.</typeparam>
	public sealed class ElementDecorationContext<T> where T : IElement
	{
		readonly IServiceRepository _repository;

		public ElementDecorationContext(IServiceRepository repository) => _repository = repository;

		/// <summary>
		/// Configures the container's <see cref="IElement"/> to use the decorated type when the provided specification is
		/// satisfied.
		/// </summary>
		/// <param name="specification">The specification to use for determining which IElement to use.  When this
		/// specification evaluates to true, the currently configured context type will be used.  Otherwise, the previous
		/// IElement will be utilized instead.</param>
		/// <returns>The configured repository.</returns>
		public IServiceRepository When(ISpecification<TypeInfo> specification)
			=> new ConditionalElementDecoration<T>(specification).Get(_repository);
	}
}