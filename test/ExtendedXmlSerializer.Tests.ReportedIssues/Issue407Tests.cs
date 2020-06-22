using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Shared;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue407Tests
	{
		[Fact]
		public void Verify()
		{
			var type = new PathogenType{ Code = 'f', Description = "Fungus" };
			var instance = new List<object>
			{
				new LookupTable<PathogenDto>
				{
					Models = new List<PathogenDto>
					{
						new Pathogen{ PathogenType = type }
					}
				},
				new LookupTable<PathogenTypeDto>
				{
					Models = new List<PathogenTypeDto> { type }
				}
			};

			var serializer = new ConfigurationContainer().EnableReferences().ForTesting();

			var cycled = serializer.Cycle(instance);
			var expected = cycled[1].To<LookupTable<PathogenTypeDto>>().Models.Only();

			cycled[0].To<LookupTable<PathogenDto>>()
			         .Models[0].PathogenType.Should()
			         .BeSameAs(expected);

		}

		sealed class LookupTable<T>
		{
			public List<T> Models { get; set; } = new List<T>();
		}
	}
}