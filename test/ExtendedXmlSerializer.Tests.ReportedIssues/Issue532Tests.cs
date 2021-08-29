using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using JetBrains.Annotations;
using System.Reflection;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue532Tests
	{
		[Fact]
		public void Verify()
		{
			var sut = new ConfigurationContainer().EnableImplicitTyping(typeof(Root))
			                                      .Type<Status?>()
			                                      .Register()
			                                      .Converter()
			                                      .Using(NullableStatusConverter.Default)
			                                      .Type<bool?>()
			                                      .Register()
			                                      .Converter()
			                                      .Using(NullableBoolConverter.Default)
			                                      .Create()
			                                      .ForTesting();

			{
				var instance = new Root { On = true, Current = null };
				sut.Assert(instance, @"<?xml version=""1.0"" encoding=""utf-8""?><Issue532Tests-Root><On>1</On></Issue532Tests-Root>");
				sut.Cycle(instance).Should().BeEquivalentTo(instance);
			}

			{
				var instance = new Root { On = false, Current = Status.Ok };
				sut.Assert(instance, @"<?xml version=""1.0"" encoding=""utf-8""?><Issue532Tests-Root><On>0</On><Current>0</Current></Issue532Tests-Root>");
				sut.Cycle(instance).Should().BeEquivalentTo(instance);
			}

			{
				var instance = new Root { On = null, Current = Status.Not };
				sut.Assert(instance, @"<?xml version=""1.0"" encoding=""utf-8""?><Issue532Tests-Root><Current>1</Current></Issue532Tests-Root>");
				sut.Cycle(instance).Should().BeEquivalentTo(instance);
			}

			{
				var instance = new Root { On = null, Current = null };
				sut.Assert(instance, @"<?xml version=""1.0"" encoding=""utf-8""?><Issue532Tests-Root />");
				sut.Cycle(instance).Should().BeEquivalentTo(instance);
			}
		}

		public enum Status
		{
			Ok, Not
		}

		sealed class Root
		{
			public bool? On { [UsedImplicitly] get; set; }

			public Status? Current { [UsedImplicitly] get; set; }
		}

		private class NullableStatusConverter : IConverter<Status?>
		{
			public static readonly NullableStatusConverter Default = new NullableStatusConverter();

			public bool IsSatisfiedBy(TypeInfo parameter)
				=> parameter.AsType() == typeof(Status?);

			public Status? Parse(string data) => string.IsNullOrEmpty(data) ? null : (Status)int.Parse(data);

			public string Format(Status? instance) => instance != null ? ((int)instance).ToString() : null;
		}

		private class NullableBoolConverter : IConverter<bool?>
		{
			public static readonly NullableBoolConverter Default = new();

			public bool IsSatisfiedBy(TypeInfo parameter) => parameter.AsType() == typeof(bool?);

			public bool? Parse(string data) => data switch
			{
				"0" => false,
				"1" => true,
				_ => null
			};

			public string Format(bool? instance) => instance switch
			{
				true => "1",
				false => "0",
				_ => null
			};
		}
	}
}