using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue197Tests
	{
		[Fact]
		void Verify()
		{
			var btn = new Button { Name = "Testing?", DialogResult = DialogResult.Abort };
			var serializer = new ConfigurationContainer().Extend(FallbackSerializationExtension.Default)
			                                             .Create()
			                                             .ForTesting();

			var button = serializer.Cycle(btn);
			button.Name.Should()
			      .Be(btn.Name);
			button.DialogResult.ShouldBeEquivalentTo(btn.DialogResult);

		}

		sealed class Button
		{
			public string Name { get; set; }

			public DialogResult DialogResult { get; set; } = DialogResult.None;

			public Cursor Cursor { get; set; }
		}

		sealed class Cursor
		{
			Cursor() {}
		}

		public enum DialogResult
		{
			None,
			OK,
			Cancel,
			Abort,
			Retry,
			Ignore,
			Yes,
			No,
		}
	}
}
