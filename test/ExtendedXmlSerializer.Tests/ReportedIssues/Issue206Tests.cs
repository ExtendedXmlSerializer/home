using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Types.Sources;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
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

			var subject = new[] {new TravelFile {Name = "Hello World!", Participants = new[]
			{
				new Participant{ParticipantId = 679556, Name = "xxxx"}, 
				new Participant{ParticipantId = 679557, Name = "xxx"}
			}.ToList()}};
			support.Deserialize<TravelFile[]>(@"<?xml version=""1.0"" encoding=""utf-8""?><ArrayOfTravelFile><TravelFile Name=""Hello World!""><Participants>
         <Participant>
            <ParticipantId>679556</ParticipantId>
            <Name>xxxx</Name>
         </Participant>
         <Participant>
            <ParticipantId>679557</ParticipantId>
            <Name>xxx</Name>
</Participant>
      </Participants></TravelFile></ArrayOfTravelFile>")
			       .ShouldBeEquivalentTo(subject);
		}

		public sealed class TravelFile
		{

			public List<Participant> Participants { get; set; }

			public string Name { get; set; }
		}

		public class Participant
		{
			public int ParticipantId { get; set; }


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