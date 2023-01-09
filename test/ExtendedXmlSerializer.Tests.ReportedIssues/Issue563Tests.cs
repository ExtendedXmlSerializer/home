using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using JetBrains.Annotations;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue563Tests
	{
		[Fact]
		public void Verify()
		{
			var subject  = new ConfigurationContainer().EnableReferences().Create().ForTesting();
			var instance = new Subject();
			subject.Cycle(instance).Should().BeEquivalentTo(instance);
		}

		sealed class Subject
		{
			// ReSharper disable once CollectionNeverUpdated.Local
			public List<byte[]> ParentKey { get; set; } = new ();
			public byte[] Id { get; [UsedImplicitly] set; }

			[UsedImplicitly]
			public List<byte[]> Key
			{
				get
				{
					List<byte[]> result = new(ParentKey);
					result.Add(Id);
					return result;
				}
			}
		}
	}

}