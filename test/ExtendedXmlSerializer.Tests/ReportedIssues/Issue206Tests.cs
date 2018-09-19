using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Types.Sources;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue206Tests
	{
		[Fact]
		void Verify()
		{
			var types = new PublicNestedTypes<Issue206Tests>();
			var support = new ConfigurationContainer().EnableImplicitTyping(types.Concat(types.Select(x => x.MakeArrayType())))
			                                          .Type<TravelFile[]>()
			                                          .Name("ArrayOfTravelFile")
			                                          .Create()
			                                          .ForTesting();

			var subject = new[] {new TravelFile {Name = "Hello World!"}};
			support.Deserialize<TravelFile[]>(@"<?xml version=""1.0"" encoding=""utf-8""?><ArrayOfTravelFile><TravelFile Name=""Hello World!"" /></ArrayOfTravelFile>")
			       .ShouldBeEquivalentTo(subject);
		}

		public sealed class TravelFile
		{
			public string Name { get; set; }
		}

		/*sealed class Extension : ISerializerExtension
		{
			public IServiceRepository Get(IServiceRepository parameter)
				=> parameter.Decorate<ArrayElement>(new DelegatedSpecification<TypeInfo>(x => x.IsArray));

			public void Execute(IServices parameter) {}



			sealed class ArrayElement : IElement
			{
				readonly IIdentities _identities;
				readonly IIdentityStore _store;
				readonly ICollectionItemTypeLocator _locator;

				public ArrayElement(IIdentities identities, IIdentityStore store)
					: this(identities, store, CollectionItemTypeLocator.Default) {}

				public ArrayElement(IIdentities identities, IIdentityStore store, ICollectionItemTypeLocator locator)
				{
					_identities = identities;
					_store = store;
					_locator  = locator;
				}

				public IWriter Get(TypeInfo parameter)
				{
					var element = _identities.Get(_locator.Get(parameter));
					var identity = _store.Get($"ArrayOf{element.Name}", element.Identifier);
					return new ArrayIdentity(identity).Adapt();
				}
			}

			sealed class ArrayIdentity : IWriter<Array>
			{
				readonly IIdentity _identity;

				public ArrayIdentity(IIdentity identity) => _identity = identity;

				public void Write(IFormatWriter writer, Array instance)
				{
					writer.Start(_identity);
				}
			}

		}*/

	}
}