using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	sealed class UnknownContentHandlingExtension : ISerializerExtension
	{
		readonly Action<IFormatReader> _action;

		public UnknownContentHandlingExtension(Action<IFormatReader> action) => _action = action;

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterInstance(_action)
			            .Decorate<IInnerContentServices, Services>();

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class Services : IInnerContentServices
		{
			readonly IInnerContentServices _services;
			readonly IClassification       _classification;
			readonly Action<IFormatReader> _missing;

			public Services(IInnerContentServices services, IClassification classification,
			                Action<IFormatReader> missing)
			{
				_services       = services;
				_classification = classification;
				_missing        = missing;
			}

			public bool IsSatisfiedBy(IInnerContent parameter)
			{
				var previous = _services.IsSatisfiedBy(parameter);
				if (previous)
				{
					var reader = parameter.Get();
					var type   = _classification.Get(reader);
					if (type == null)
					{
						_missing(reader);
						return false;
					}
				}

				return previous;
			}

			public void Handle(IInnerContent contents, IMemberSerializer member)
			{
				_services.Handle(contents, member);
			}

			public void Handle(IListInnerContent contents, IReader reader)
			{
				_services.Handle(contents, reader);
			}

			public string Get(IFormatReader parameter) => _services.Get(parameter);

			public IReader Create(TypeInfo classification, IInnerContentHandler handler)
				=> _services.Create(classification, new Handler(handler, _missing));
		}

		sealed class Handler : IInnerContentHandler
		{
			readonly IInnerContentHandler  _handler;
			readonly Action<IFormatReader> _command;

			public Handler(IInnerContentHandler handler, Action<IFormatReader> command)
			{
				_handler = handler;
				_command = command;
			}

			public bool IsSatisfiedBy(IInnerContent parameter)
			{
				var result = _handler.IsSatisfiedBy(parameter);
				if (!result)
				{
					var reader = parameter.Get();
					switch (reader.Identifier)
					{
						case "http://www.w3.org/2000/xmlns/":
						case "https://extendedxmlserializer.github.io/v2":
							break;
						default:
							_command(reader);
							break;
					}
				}

				return result;
			}
		}
	}
}