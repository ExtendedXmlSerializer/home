using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using FluentAssertions;
using System.Linq;
using System.Reflection;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue238Tests
	{
		[Fact]
		void Verify()
		{
			var key = typeof(Subject).GetMember(nameof(Subject.Message)).Single();

			new ConfigurationContainer().Type<Subject>()
			                            .MemberBy(key.As<string>())
			                            .AsValid<ISource<MemberInfo>>()
			                            .Get()
			                            .Should()
			                            .BeSameAs(key);
		}

		sealed class Subject
		{
			public string Message { get; set; }
		}
	}
}
