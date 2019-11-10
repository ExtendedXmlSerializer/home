using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class OptimizedWriters : IFormatWriters<System.Xml.XmlWriter>
	{
		readonly ISpecification<object>               _specification;
		readonly IFormatWriters<System.Xml.XmlWriter> _factory;
		readonly IObjectIdentifiers                   _identifiers;
		readonly IRootInstances                       _instances;

		public OptimizedWriters(IFormatWriters<System.Xml.XmlWriter> factory, IObjectIdentifiers identifiers,
		                        IRootInstances instances)
			: this(new InstanceConditionalSpecification(), factory, identifiers, instances) {}

		// ReSharper disable once TooManyDependencies
		public OptimizedWriters(ISpecification<object> specification, IFormatWriters<System.Xml.XmlWriter> factory,
		                        IObjectIdentifiers identifiers, IRootInstances instances)
		{
			_specification = specification;
			_factory       = factory;
			_identifiers   = identifiers;
			_instances     = instances;
		}

		public IFormatWriter Get(System.Xml.XmlWriter parameter)
			=> new OptimizedNamespaceXmlWriter(_factory.Get(parameter),
			                                   new IdentifierCommand(_identifiers,
			                                                         _specification.Fix(parameter),
			                                                         _instances.Build(parameter)));
	}
}