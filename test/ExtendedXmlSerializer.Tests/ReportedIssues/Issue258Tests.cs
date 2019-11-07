using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Xml;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue258Tests
	{
		[Fact]
		void Verify()
		{
			const string data =
				@"<?xml version=""1.0"" encoding=""utf-8""?><List xmlns:ns1=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests"" xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:arguments=""ns1:Issue258Tests-Subject"" xmlns=""https://extendedxmlserializer.github.io/system""><Capacity>4</Capacity><ns1:Issue258Tests-Subject><Foo>Doesn't Exist!</Foo><Message>Hello World!</Message></ns1:Issue258Tests-Subject></List>";

			var command = new Command();

			new ConfigurationContainer().EnableReaderContext()
			                            .WithUnknownContent()
			                            .Call(command.Execute)
			                            .Create()
			                            .Deserialize<List<Subject>>(data)
			                            .Only()
			                            .Message.Should()
			                            .Be("Hello World!");
			command.Captured.Name.Should()
			       .Be("https://extendedxmlserializer.github.io/system:Foo");
			command.Captured.Line.Should()
			       .BeGreaterThan(0);
			command.Captured.Position.Should()
			       .BeGreaterThan(0);
			command.Captured.Type.Should()
			       .Be(typeof(Subject));
		}

		[Fact]
		void VerifyWithoutInvalid()
		{
			const string data =
				@"<?xml version=""1.0"" encoding=""utf-8""?><List xmlns:ns1=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests"" xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:arguments=""ns1:Issue258Tests-Subject"" xmlns=""https://extendedxmlserializer.github.io/system""><Capacity>4</Capacity><ns1:Issue258Tests-Subject><Message>Hello World!</Message></ns1:Issue258Tests-Subject></List>";

			var command = new Command();

			new ConfigurationContainer().EnableReaderContext()
			                            .WithUnknownContent()
			                            .Call(command.Execute)
			                            .Create()
			                            .Deserialize<List<Subject>>(data)
			                            .Only()
			                            .Message.Should()
			                            .Be("Hello World!");
			command.Captured.Should()
			       .BeNull();
		}

		sealed class CapturedInfo
		{
			public CapturedInfo(string name, uint line, uint position, Type type)
			{
				Name     = name;
				Line     = line;
				Position = position;
				Type     = type;
			}

			public string Name { get; }
			public uint Line { get; }
			public uint Position { get; }
			public Type Type { get; }
		}

		sealed class Command : ICommand<IFormatReader>
		{
			public CapturedInfo Captured { get; private set; }

			public void Execute(IFormatReader parameter)
			{
				var content = ContentsHistory.Default.Get(parameter)
				                             .Peek();

				var lineInfo = parameter.Get()
				                        .To<IXmlLineInfo>();
				Captured = new CapturedInfo(IdentityFormatter.Default.Get(parameter), (uint)lineInfo.LineNumber,
				                            (uint)lineInfo.LinePosition, content.Current.GetType());

				parameter.Content();
			}
		}

		sealed class Subject
		{
			public string Message { get; set; }
		}
	}
}