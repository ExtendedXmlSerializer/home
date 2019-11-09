using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using FluentAssertions;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Xml;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue256Tests
	{
		[Fact]
		void Verify()
		{
			var names = new List<string>();
			var data =
				@"<?xml version=""1.0"" encoding=""utf-8""?><Issue256Tests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Invalid>Hello World!</Invalid><Message>Hello World!</Message></Issue256Tests-Subject>";
			new ConfigurationContainer().WithUnknownContent()
			                            .Call(reader => names.Add(IdentityFormatter.Default.Get(reader)))
			                            .Create()
			                            .Deserialize<Subject>(data);
			names.Should()
			     .HaveCount(1)
			     .And.Subject.Should()
			     .Contain("clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests:Invalid");
		}

		[Fact]
		void VerifyContinue()
		{
			const string data =
				@"<?xml version=""1.0"" encoding=""utf-8""?><Issue256Tests-SubjectContinue xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Foo>Doesn't Exist!</Foo><Bar>Hello World!</Bar></Issue256Tests-SubjectContinue>";

			var command = new Command();

			new ConfigurationContainer().EnableReaderContext()
			                            .WithUnknownContent()
			                            .Call(command.Execute)
			                            .Create()
			                            .Deserialize<SubjectContinue>(data)
			                            .Bar.Should()
			                            .Be("Hello World!");
			command.Captured.Name.Should()
			       .Be("clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests:Foo");
			command.Captured.Line.Should()
			       .BeGreaterThan(0);
			command.Captured.Position.Should()
			       .BeGreaterThan(0);
			command.Captured.Type.Should()
			       .Be(typeof(SubjectContinue));
		}

		readonly struct CapturedInfo
		{
			// ReSharper disable once TooManyDependencies
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
				var content =
					ContentsHistory.Default.Get(parameter)
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
			[UsedImplicitly]
			public string Message { get; set; }
		}

		sealed class SubjectContinue
		{
			public string Bar { get; [UsedImplicitly] set; }
		}
	}
}