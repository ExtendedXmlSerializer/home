using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Members;
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
			            .Decorate<IInnerContentServices, Services>()
			            .Decorate<IInstanceMemberSerializations, InstanceMemberSerializations>();

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class InstanceMemberSerializations : IInstanceMemberSerializations
		{
			readonly IInstanceMemberSerializations _previous;
			readonly IInnerContentServices         _services;

			public InstanceMemberSerializations(IInstanceMemberSerializations previous, IInnerContentServices services)
			{
				_previous = previous;
				_services = services;
			}

			public IInstanceMemberSerialization Get(TypeInfo parameter)
				=> new InstanceMemberSerialization(_previous.Get(parameter), _services);

			sealed class InstanceMemberSerialization : IInstanceMemberSerialization
			{
				readonly IInstanceMemberSerialization _previous;
				readonly IInnerContentServices        _services;

				public InstanceMemberSerialization(IInstanceMemberSerialization previous,
				                                   IInnerContentServices services)
				{
					_previous = previous;
					_services = services;
				}

				public IMemberSerialization Get(IInnerContent parameter)
				{
					var content       = parameter.Get();
					var key           = _services.Get(content);
					var serialization = _previous.Get(parameter);
					var serializer    = serialization.Get(key);
					var result = serializer is PropertyMemberSerializer && !content.IsSatisfiedBy(serializer.Profile)
						             ? null
						             : serialization;
					return result;
				}

				public IMemberSerialization Get(object parameter) => _previous.Get(parameter);
			}
		}

		sealed class Services : IInnerContentServices
		{
			readonly IInnerContentServices _services;
			readonly Action<IFormatReader> _missing;

			public Services(IInnerContentServices services, Action<IFormatReader> missing)
			{
				_services = services;
				_missing  = missing;
			}

			public bool IsSatisfiedBy(IInnerContent parameter) => _services.IsSatisfiedBy(parameter);

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