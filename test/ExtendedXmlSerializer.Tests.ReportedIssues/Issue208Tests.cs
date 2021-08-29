﻿using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue208Tests
	{
		[Fact]
		void Verify()
		{
			var container = new ConfigurationContainer().EnableThreadProtection()
			                                            .Create()
			                                            .ForTesting();

			var subject = "Hello World!";

			var loop = Parallel.For(0, 1000,
			                        _ => container.Cycle(subject)
			                                          .Should()
			                                          .Be(subject));
			while (!loop.IsCompleted)
			{
				Thread.Sleep(100);
			}
		}
	}
}