using ExtendedXmlSerializer.Configuration;
using FluentAssertions;
using LightInject;
using System.Xml;
using Xunit;
// ReSharper disable All

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
#nullable enable
	public sealed class Issue485Tests_Extended
	{
		[Fact]
		public void Verify()
		{
			var serializer = new ConfigurationContainer().UseAutoFormatting()
			                                             .EnableImmutableTypes()
			                                             .EnableParameterizedContent()
			                                             .Create();
			var settings = new XmlWriterSettings { Indent = true };

			var instance = new U("ciao", null);
			var xml      = serializer.Serialize(settings, instance);

			var deserialized = serializer.Deserialize<U>(xml);
			deserialized.Member.Should().BeNull();
		}

		[Fact]
		public void VerifyAlternate()
		{
			var serializer = new ConfigurationContainer().UseAutoFormatting()
			                                             .EnableImmutableTypes()
			                                             .EnableParameterizedContentWithPropertyAssignments()
			                                             .Create();
			var settings = new XmlWriterSettings { Indent = true };

			var instance = new U("ciao", null);
			var xml      = serializer.Serialize(settings, instance);

			var deserialized = serializer.Deserialize<U>(xml);
			deserialized.Member.Should().BeNull();
		}

		class U
		{
			public string Id { get; }
			public T? Member { get; }

			public U(string id, T? member)
			{
				Id     = id;
				Member = member;
			}
		}

		class T
		{
			public string Name { get; }
			public ImmutableList<int> List { get; }

			public T(string name, ImmutableList<int> list)
			{
				Name = name;
				List = list;
			}
		}

	}
	#nullable restore
}
