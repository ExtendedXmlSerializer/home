using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue386Tests
	{
		[Fact]
		void Verify()
		{
			var instance = LogMessageSeverity.Debug | LogMessageSeverity.Informational;
			var serializer = new ConfigurationContainer().Type<LogMessageSeverity>()
			                                             .Register()
			                                             .Converter()
			                                             .Using(Converter.Default)
			                                             .ForTesting();
			serializer.Cycle(instance).Should().Be(instance);
		}

		sealed class Converter : Converter<LogMessageSeverity>
		{
			public static Converter Default { get; } = new Converter();

			Converter() : base(x => int.TryParse(x, out var bits)
				                        ? (LogMessageSeverity)bits
				                        : LogMessageSeverity.Informational,
			                   severity => ((int)severity).ToString()) {}
		}

		[Flags]
		enum LogMessageSeverity
		{
			Debug         = 1,
			Informational = 2,
			Critical      = 4
		}
	}
}