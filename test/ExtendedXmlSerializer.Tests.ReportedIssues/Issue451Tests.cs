using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ExtensionModel.Instances;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue451Tests
	{
		[Fact]
		public void Verify()
		{
			var serializer = new ConfigurationContainer().Type<Subject>()
			                                             .WithInterceptor(Interceptor.Default)
			                                             .Create()
			                                             .ForTesting();

			var instance = new Subject();

			instance.Count.Should().Be(0);
			const string content =
				@"<?xml version=""1.0"" encoding=""utf-8""?><Issue451Tests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests.ReportedIssues""><Count>1</Count></Issue451Tests-Subject>";
			serializer.Assert(instance, content);
			instance.Count.Should().Be(1);

			var cycled = serializer.Deserialize<Subject>(content);
			cycled.Should().BeOfType<ActivatedSubject>();
			cycled.Count.Should().Be(2);
		}

		sealed class Interceptor : ISerializationInterceptor<Subject>
		{
			public static Interceptor Default { get; } = new Interceptor();

			Interceptor() {}

			public Subject Serializing(IFormatWriter writer, Subject instance)
			{
				instance.Count++;
				return instance;
			}

			public Subject Activating(Type instanceType) => new ActivatedSubject();

			public Subject Deserialized(IFormatReader reader, Subject instance)
			{
				instance.Count++;
				return instance;
			}
		}

		sealed class ActivatedSubject : Subject {}

		class Subject
		{
			public uint Count { get; set; }
		}
	}
}