using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using FluentAssertions;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue271Tests
	{
		[Fact]
		void Verify()
		{
			var serializer = new ConfigurationContainer().EnableImplicitTyping(typeof(Entity), typeof(Entity2))
			                                             .Create();

			// language=XML
			const string content1 =
				@"<?xml version=""1.0"" encoding=""utf-8""?>
				<Issue271Tests-Entity>
					<Name>Foo</Name>
					<Show>false</Show>
					<Children>
						<Issue271Tests-Entity>
							<Name>Bar</Name>
							<Show>false</Show>
						</Issue271Tests-Entity>
						<Issue271Tests-Entity>
							<Name>Jim</Name>
							<Show>true</Show>
						</Issue271Tests-Entity>
					</Children>
				</Issue271Tests-Entity>";

			serializer.Deserialize<Entity>(content1)
			          .Children.Should()
			          .HaveCount(0);
		}

		[Fact]
		void VerifyThrow()
		{
			var serializer = new ConfigurationContainer().EnableImplicitTyping(typeof(Entity), typeof(Entity2))
			                                             .WithUnknownContent()
			                                             .Throw()
			                                             .Create();

			// language=XML
			const string content1 =
				@"<?xml version=""1.0"" encoding=""utf-8""?>
				<Issue271Tests-Entity>
					<Name>Foo</Name>
					<Show>false</Show>
					<Children>
						<Issue271Tests-Entity>
							<Name>Bar</Name>
							<Show>false</Show>
						</Issue271Tests-Entity>
						<Issue271Tests-Entity>
							<Name>Jim</Name>
							<Show>true</Show>
						</Issue271Tests-Entity>
					</Children>
				</Issue271Tests-Entity>";

			serializer.Invoking(x => x.Deserialize<Entity>(content1))
			          .ShouldThrow<XmlException>()
			          .WithMessage("Unknown/invalid member encountered: 'Show'. Line 4, position 7.");
		}

		[Fact]
		void VerifyContinue()
		{
			var serializer = new ConfigurationContainer().EnableImplicitTyping(typeof(Entity), typeof(Entity2))
			                                             .WithUnknownContent()
			                                             .Continue()
			                                             .Create();

			// language=XML
			const string content1 =
				@"<?xml version=""1.0"" encoding=""utf-8""?>
				<Issue271Tests-Entity>
					<Name>Foo</Name>
					<Show>false</Show>
					<Children>
						<Issue271Tests-Entity>
							<Name>Bar</Name>
							<Show>false</Show>
						</Issue271Tests-Entity>
						<Issue271Tests-Entity>
							<Name>Jim</Name>
							<Show>true</Show>
						</Issue271Tests-Entity>
					</Children>
				</Issue271Tests-Entity>";

			serializer.Deserialize<Entity>(content1)
			          .Children.Should()
			          .HaveCount(2);
		}

		[XmlInclude(typeof(Entity2))]
		public class Entity
		{
			public string Name { get; set; }

			[XmlIgnore]
			public virtual bool Show
			{
				get => true;
				// ReSharper disable once ValueParameterNotUsed
				set {}
			}

			// ReSharper disable once CollectionNeverUpdated.Global
			public List<Entity> Children { get; } = new List<Entity>();
		}

		public class Entity2 : Entity
		{
			[DataMember]
			public override bool Show { get; set; }
		}
	}
}