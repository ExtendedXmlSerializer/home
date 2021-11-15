using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue565Tests_Extended_II
	{
		[Fact]
		public void Verify()
		{
			List<TestSerialization> instance = new List<TestSerialization>();
			for (int i = 0; i < 2000; i++)
			{
				var newElement = new TestSerialization();
				if (i % 2 == 0)
				{
					newElement.MyString  = "**********";
					newElement.MyDouble  = Double.MaxValue;
					newElement.MyBool    = true;
					newElement.MyInteger = i;
				}
				else
				{
					newElement.MyString  = "----------";
					newElement.MyDouble  = Double.MinValue;
					newElement.MyBool    = false;
					newElement.MyInteger = i;
				}
				instance.Add(newElement);
			}

			var serializer = new ConfigurationContainer().EnableImplicitTyping(typeof(List<TestSerialization>))
			                                             .Create();

			serializer.Cycle(instance).Should().BeEquivalentTo(instance);



		}

		[Serializable]
		public class TestSerialization
		{
			public string MyString { get; set; }

			public bool MyBool { get; set; }

			public double MyDouble { get; set; }

			public int MyInteger { get; set; }
		}
	}
}
