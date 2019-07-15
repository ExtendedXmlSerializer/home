using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue240Tests
	{
		[Fact]
		void Verify()
		{
			/*const string content =
				@"<?xml version=""1.0"" encoding=""utf-8""?><Issue240Tests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Messages><Capacity>2</Capacity><string xmlns=""https://extendedxmlserializer.github.io/system"">Hello</string><string xmlns=""https://extendedxmlserializer.github.io/system"">World</string></Messages></Issue240Tests-Subject>";

			var temp = new ConfigurationContainer()/*.Extend(Extension.Default)#1#
			                            .Create()
			                            .ForTesting()
			                            .Deserialize<Subject>(content);
			Debugger.Break();*/


		}

		sealed class Subject
		{
			public List<string> Messages { get; set; }
		}

		sealed class Extension : ISerializerExtension
		{
			public static Extension Default { get; } = new Extension();

			Extension() {}

			public IServiceRepository Get(IServiceRepository parameter)
				=> parameter.Decorate<IInnerContentServices, Services>();

			void ICommand<IServices>.Execute(IServices parameter) {}

			sealed class Services : IInnerContentServices
			{
				readonly IInnerContentServices _services;

				public Services(IInnerContentServices services)
				{
					_services = services;
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
					=> _services.Create(classification, new Handler(handler));
			}

			sealed class Handler : IInnerContentHandler
			{
				readonly IInnerContentHandler _handler;

				public Handler(IInnerContentHandler handler) => _handler = handler;

				public bool IsSatisfiedBy(IInnerContent parameter)
				{
					var result = _handler.IsSatisfiedBy(parameter);

					if (!result) {}

					return result;
				}
			}
		}
	}
}