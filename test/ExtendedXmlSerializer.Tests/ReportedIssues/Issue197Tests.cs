using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System;
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

		[Fact]
		void VerifyIgnore()
		{
			new ConfigurationContainer().Extend(FallbackSerializationExtension.Default)
			                            .Type<Button>().Member(m => m.AutoSize).Ignore()
			                            .Create()
			                            .ForTesting()
			                            .Assert(new Button(),
			                @"<?xml version=""1.0"" encoding=""utf-8""?><Issue197Tests-Button xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><DialogResult>None</DialogResult></Issue197Tests-Button>");
			new ConfigurationContainer().Extend(FallbackSerializationExtension.Default)
			                            .Type<Button>()
			                            .Create()
			                            .ForTesting()
			                            .Assert(new Button(),
			                                    @"<?xml version=""1.0"" encoding=""utf-8""?><Issue197Tests-Button xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><AutoSize>false</AutoSize><DialogResult>None</DialogResult></Issue197Tests-Button>");
		}

		[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
		public sealed class ButtonExPropertyAttribute : System.Attribute { }

		class ButtonExBase : Button
		{
			[ButtonExProperty]
			public int FirstPropButtonExBase { get; set; }
			public int SecondPropButtonExBase { get; set; }
		}

		class ButtonEx : ButtonExBase
		{
			[ButtonExProperty]
			public int PropButtonEx { get; set; }
		}

		class ButtonBase : Control
		{
			public override bool AutoSize
			{
				get => base.AutoSize;
				set => base.AutoSize = value;
			}
		}

		class Control
		{
			public virtual bool AutoSize { get; set; }
		}


		class Button : ButtonBase
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
