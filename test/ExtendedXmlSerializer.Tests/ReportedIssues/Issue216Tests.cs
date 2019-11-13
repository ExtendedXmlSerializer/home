using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using JetBrains.Annotations;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue216Tests
	{
		[Fact]
		void Verify()
		{
			var instance = new Foo {Baz = {"hello"}};
			var cycle = new ConfigurationContainer().Emit(EmitBehaviors.WhenModified)
			                                        .Create()
			                                        .Cycle(instance);
			cycle.Should().BeEquivalentTo(instance);
		}

		[Fact]
		void VerifyAlways()
		{
			var instance = new Foo();
			var cycle = new ConfigurationContainer().Emit(EmitBehaviors.Always)
			                                        .Create()
			                                        .Cycle(instance);
			cycle.Should().BeEquivalentTo(instance);
		}

		[Fact]
		void Emit()
		{
			var instance = new Foo {Baz = {"hello"}};
			new ConfigurationContainer().Emit(EmitBehaviors.WhenModified)
			                            .Create()
			                            .ForTesting()
			                            .Assert(instance,
			                                    @"<?xml version=""1.0"" encoding=""utf-8""?><Issue216Tests-Foo xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Baz><Capacity>4</Capacity><string xmlns=""https://extendedxmlserializer.github.io/system"">hello</string></Baz></Issue216Tests-Foo>");
		}

		[Fact]
		void EmitEmpty()
		{
			var instance = new Foo();
			new ConfigurationContainer().Emit(EmitBehaviors.WhenModified)
			                            .Create()
			                            .ForTesting()
			                            .Assert(instance,
			                                    @"<?xml version=""1.0"" encoding=""utf-8""?><Issue216Tests-Foo xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests"" />");
		}

		[Fact]
		void EmitAlways()
		{
			var instance = new Foo {Baz = {"hello"}};
			new ConfigurationContainer().Emit(EmitBehaviors.Always)
			                            .Create()
			                            .ForTesting()
			                            .Assert(instance,
			                                    @"<?xml version=""1.0"" encoding=""utf-8""?><Issue216Tests-Foo xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Baz><Capacity>4</Capacity><string xmlns=""https://extendedxmlserializer.github.io/system"">hello</string></Baz></Issue216Tests-Foo>");
		}

		[Fact]
		void EmitAlwaysEmpty()
		{
			var instance = new Foo();
			new ConfigurationContainer().Emit(EmitBehaviors.Always)
			                            .Create()
			                            .ForTesting()
			                            .Assert(instance,
			                                    @"<?xml version=""1.0"" encoding=""utf-8""?><Issue216Tests-Foo xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Baz><Capacity>0</Capacity></Baz></Issue216Tests-Foo>");
		}

		[Fact]
		void VerifyReadOnlyDoesNotEmit()
		{
			new ConfigurationContainer().Emit(EmitBehaviors.Always)
			                            .Create()
			                            .ForTesting()
			                            .Assert(new Subject {Message = "Hello World!"},
			                                    @"<?xml version=""1.0"" encoding=""utf-8""?><Issue216Tests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Message>Hello World!</Message></Issue216Tests-Subject>");
		}

		[Fact]
		void VerifyReadOnlyDoesNotEmitAssigned()
		{
			new ConfigurationContainer().Emit(EmitBehaviors.WhenModified)
			                            .Create()
			                            .ForTesting()
			                            .Assert(new Subject {Message = "Hello World!"},
			                                    @"<?xml version=""1.0"" encoding=""utf-8""?><Issue216Tests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Message>Hello World!</Message></Issue216Tests-Subject>");
		}

		sealed class Subject
		{
			string _message;

			public string Message
			{
				[UsedImplicitly] get => _message;
				set
				{
					_message  = value;
					Formatted = $"Yo: {_message}";
				}
			}

			public string Formatted { [UsedImplicitly] get; private set; }
		}

		public class Foo
		{
			// ReSharper disable once CollectionNeverQueried.Global
			public List<string> Baz { get; } = new List<string>();
		}
	}
}