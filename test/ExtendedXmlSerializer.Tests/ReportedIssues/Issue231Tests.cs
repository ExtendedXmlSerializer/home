using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using FluentAssertions;
using System;
using System.IO;
using System.Text;
using System.Xml;
using Xunit;
using XmlWriter = System.Xml.XmlWriter;

// ReSharper disable All

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue231Tests
	{
		[Fact]
		void Verify()
		{
			var subject = new Subject {Property = "Hello World"};

			var serializer = new ConfigurationContainer().EnableImplicitTypingFromPublicNested<Issue231Tests>()
			                                             .Create();

			using (var xmlTextWriter = new Writer())
			{
				serializer.Serialize(xmlTextWriter, subject);
			}
		}

		[Fact]
		void CustomPrefixesShouldThrow()
		{
			var subject = new PatchMapping()
			{
				Source          = "File1.bin",
				Destination     = "EFI\\Microsoft\\Boot",
				TargetPartition = TargetPartition.Boot,
				Condition       = new Always(),
			};

			var serializer = new ConfigurationContainer().EnableImplicitTypingFromPublicNested<Issue231Tests>()
			                                             .Extend(Extension.Default)
			                                             .Create();

			using (var xmlTextWriter = new Writer())
			{
				Action action = () => serializer.Serialize(xmlTextWriter, subject);

				action.ShouldThrow<InvalidOperationException>()
				      .WithMessage("Nope.");
			}
		}

		sealed class Extension : ISerializerExtension
		{
			public static Extension Default { get; } = new Extension();

			Extension() {}

			public IServiceRepository Get(IServiceRepository parameter)
				=> parameter.RegisterInstance<IPrefixes>(Prefixes.Instance);

			public void Execute(IServices parameter) {}

			sealed class Prefixes : IPrefixes
			{
				public static Prefixes Instance { get; } = new Prefixes();

				Prefixes() {}

				public IPrefix Get(XmlWriter parameter) => Prefix.Instance;
			}

			sealed class Prefix : IPrefix
			{
				public static Prefix Instance { get; } = new Prefix();

				Prefix() {}

				public string Get(string parameter) => throw new InvalidOperationException("Nope.");
			}
		}

		sealed class Writer : XmlTextWriter
		{
			public Writer() : base(new MemoryStream(), Encoding.UTF8) {}
		}

		public sealed class Subject
		{
			public string Property { get; set; }
		}

		public class Always : Condition
		{
			public override bool IsSatified(int buildNumber)
			{
				return true;
			}
		}

		public abstract class Condition
		{
			public abstract bool IsSatified(int buildNumber);
		}

		public class PatchMapping
		{
			public string Source { get; set; }
			public string Destination { get; set; }
			public TargetPartition TargetPartition { get; set; }
			public Condition Condition { get; set; }
		}

		public enum TargetPartition
		{
			Boot,
			Windows,
		}
	}
}