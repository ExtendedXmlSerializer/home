using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using Xunit;
// ReSharper disable InconsistentNaming

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue152Tests
	{
		[Fact]
		public void Verify()
		{
			var serializer = new ConfigurationContainer().Create()
			                                             .ForTesting();

			var instance = new Subject();
			instance.AddChild(new Subject());
			var subject = serializer.Cycle(instance);
			subject.ShouldAllBeEquivalentTo(instance);
		}


		public sealed class Subject : IEnumerable<Subject>
		{
			Subject parent;


			[XmlElement]
			readonly List<Subject> children = new List<Subject>();

			[XmlIgnore]
			public ReadOnlyCollection<Subject> Children => children.AsReadOnly();

			public void AddChild(Subject go)
			{
				// remove old parent
				parent?.children.Remove(go);

				// set parent
				children.Add(go);
				go.parent = this;
			}


			public IEnumerator<Subject> GetEnumerator() => Children.GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}
	}
}