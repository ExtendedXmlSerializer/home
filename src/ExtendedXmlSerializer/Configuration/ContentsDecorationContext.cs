using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Content;
using System.Reflection;

namespace ExtendedXmlSerializer.Configuration
{
	/// <summary>
	/// Used to establish a fluent context for decorating the <see cref="IContents"/> component when configuring the
	/// configuration container.
	/// </summary>
	/// <typeparam name="T">The IContents type.</typeparam>
	public sealed class ContentsDecorationContext<T> where T : IContents
	{
		readonly IServiceRepository _repository;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="repository">The service repository to configure.</param>
		public ContentsDecorationContext(IServiceRepository repository) => _repository = repository;

		/// <summary>
		/// Configures the <see cref="IContents"/> to use the configured context type when the provided specification is satisfied.
		/// </summary>
		/// <typeparam name="TSpecification">The specification to use for determining which IContents to use.  When this
		/// specification evaluates to true, the currently configured context type will be used.  Otherwise, the previous
		/// IContents will be utilized instead.</typeparam>
		/// <returns>The configured repository.</returns>
		public IServiceRepository When<TSpecification>() where TSpecification : ISpecification<TypeInfo>
			=> ConditionalContentDecoration<TSpecification, T>.Default.Get(_repository);

		/// <summary>
		/// Configures the <see cref="IContents"/> to use the configured context type when the provided specification is
		/// satisfied.
		/// </summary>
		/// <param name="specification">The specification to use for determining which IContents to use.  When this
		/// specification evaluates to true, the currently configured context type will be used.  Otherwise, the previous
		/// IContents will be utilized instead.</param>
		/// <returns>The configured repository.</returns>
		public IServiceRepository When(ISpecification<TypeInfo> specification)
			=> new ConditionalContentDecoration<T>(specification).Get(_repository);

		/// <summary>
		/// Decorates <see cref="IContents"/> with the currently configured context type and returns the configured service
		/// repository.
		/// </summary>
		/// <returns>The configured repository.</returns>
		public IServiceRepository Then() => _repository.Decorate<IContents, T>();
	}
}