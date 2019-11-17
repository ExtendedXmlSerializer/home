using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Specifications;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class OptimizedWriters : IFormatWriters
	{
		readonly ISpecification<object> _specification;
		readonly IFormatWriters         _factory;
		readonly IObjectIdentifiers     _identifiers;
		readonly IRootInstances         _instances;

		[UsedImplicitly]
		public OptimizedWriters(IFormatWriters factory, IObjectIdentifiers identifiers,
		                        IRootInstances instances)
			: this(new InstanceConditionalSpecification(), factory, identifiers, instances) {}

		// ReSharper disable once TooManyDependencies
		public OptimizedWriters(ISpecification<object> specification, IFormatWriters factory,
		                        IObjectIdentifiers identifiers, IRootInstances instances)
		{
			_specification = specification;
			_factory       = factory;
			_identifiers   = identifiers;
			_instances     = instances;
		}

		public IFormatWriter Get(System.Xml.XmlWriter parameter)
			=> new OptimizedNamespaceXmlWriter(_factory.Get(parameter),
			                                   new IdentifierCommand(_identifiers, _specification.Fix(parameter),
			                                                         _instances.Build(parameter)));
	}
}