using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class InnerContentServices : IInnerContentServices
	{
		readonly IListContentsSpecification        _specification;
		readonly IInnerContentActivation           _activation;
		readonly IAlteration<IInnerContentHandler> _handler;
		readonly IInnerContentResult               _results;
		readonly IMemberHandler                    _member;
		readonly ICollectionContentsHandler        _collection;
		readonly IReaderFormatter                  _formatter;

		// ReSharper disable once TooManyDependencies
		public InnerContentServices(IListContentsSpecification specification, IInnerContentActivation activation,
		                            IAlteration<IInnerContentHandler> handler, IInnerContentResult results,
		                            IMemberHandler member, ICollectionContentsHandler collection,
		                            IReaderFormatter formatter)
		{
			_specification = specification;
			_activation    = activation;
			_handler       = handler;
			_results       = results;
			_member        = member;
			_collection    = collection;
			_formatter     = formatter;
		}

		public bool IsSatisfiedBy(IInnerContent parameter) => _specification.IsSatisfiedBy(parameter);

		public string Get(IFormatReader parameter) => _formatter.Get(parameter);

		public IReader Create(TypeInfo classification, IInnerContentHandler handler)
			=> new InnerContentReader(_activation.Get(classification), _handler.Get(handler), _results);

		public void Handle(IInnerContent contents, IMemberSerializer member) => _member.Handle(contents, member);

		public void Handle(IListInnerContent contents, IReader reader) => _collection.Handle(contents, reader);
	}
}