using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue255Tests
	{
		[Fact]
		void Verify()
		{
			var instance = new BackingFieldExample().Initialized();

			new ConfigurationContainer().Create()
			                            .ForTesting()
			                            .Cycle(instance)
			                            .ShouldBeEquivalentTo(instance);
		}

		public class BackingFieldExample
		{
			[XmlElement]
			readonly IList<string> _values = new List<string>();

			public BackingFieldExample Initialized()
			{
				AddValue("Hello");
				AddValue("World");
				return this;
			}

			public IReadOnlyCollection<string> Values => this._values.ToList().AsReadOnly();

			public void AddValue(string value) => this._values.Add(value);
		}
	}
}