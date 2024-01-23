using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue608Tests
	{
#if CORE
		public class TestClass
		{
			public DateOnly DateOfBirth { get; set; }
		}

		[Fact]
		public void Verify()
		{
			TestClass instance = new()
			{
				DateOfBirth = DateOnly.Parse("2024-01-22")
			};

			var serializer = new ConfigurationContainer().UseAutoFormatting()
			                                             .UseOptimizedNamespaces()
			                                             .Type<DateOnly>()
			                                             .Register()
			                                             .Converter()
			                                             .Using(DateOnlyConverter.Default)
			                                             .ForTesting();

			serializer.Cycle(instance).Should().BeEquivalentTo(instance);
		}

		sealed class DateOnlyConverter : ConverterBase<DateOnly>
		{
			public static DateOnlyConverter Default { get; } = new();

			DateOnlyConverter() {}

			public override DateOnly Parse(string data) => DateOnly.Parse(data);

			public override string Format(DateOnly instance) => instance.ToString();
		}
#endif
	}
}