using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using JetBrains.Annotations;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue530Tests
	{
		[Fact]
		public void Verify()
		{
			var instance = new Subject { List = new List<string>() };

			var container = new ConfigurationContainer().Emit(EmitBehaviors.Classic).Create().ForTesting();

			container.Cycle(instance).Should().BeEquivalentTo(instance);
		}

		[Fact]
		public void VerifyNotEmit()
		{
			var instance = new EmitMe()
			{
				Message    = "This is emitted",
				NotEmitted = new DoNotEmitMe { Message = "But I want to be emitted... :( :( :(" }
			};

			var container = new ConfigurationContainer().Type<DoNotEmitMe>()
			                                            .EmitWhen(_ => false)
			                                            .Member(x => x.Message)
			                                            .EmitWhen(x => x.Contains("emitted"))
			                                            .Create()
			                                            .ForTesting();
			container.Assert(instance,
			                 @"<?xml version=""1.0"" encoding=""utf-8""?><Issue530Tests-EmitMe xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests.ReportedIssues""><Message>This is emitted</Message></Issue530Tests-EmitMe>");
		}

		[Fact]
		public void VerifyNullable()
		{
			var instance = new TestClass { ValueList = new List<double>() };

			var sut = new ConfigurationContainer().UseAutoFormatting()
			                                      .Emit(EmitBehaviors.Always)
			                                      .Create()
			                                      .ForTesting();

			sut.Cycle(instance).Should().BeEquivalentTo(instance);
		}

		public class TestClass
		{
			public double? Value1;

			public List<double> ValueList;
		}

		sealed class Subject
		{
			[UsedImplicitly]
			public List<string> List { get; set; }
		}

		sealed class EmitMe
		{
			public string Message { get; set; }

			public DoNotEmitMe NotEmitted { get; set; }
		}

		sealed class DoNotEmitMe
		{
			public string Message { get; set; }
		}
	}
}