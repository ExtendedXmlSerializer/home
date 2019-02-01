using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue232Tests
	{
		[Fact]
		void Verify()
		{
			var outList = new List<ClassA>
			{
				new ClassA()
				{
					blah = "Test1",
					InterfaceConcreteTypeA = new ClassB
					{
						TestInterfaceProperty = "Blah1", TestConcretePropertyB = "Blah1"
					},
					InterfaceConcreteTypeB = new ClassC()
					{
						TestInterfaceProperty = "Blah2", TestConcretePropertyC = "Blah2"
					}
				},
				new ClassA()
				{
					blah = "Test2", InterfaceConcreteTypeA = new ClassB(), InterfaceConcreteTypeB = new ClassC()
				},
				new ClassA()
				{
					blah = "Test3",
					InterfaceConcreteTypeA =
						new ClassB
						{
							TestInterfaceProperty = "Blah3", TestConcretePropertyB = "Blah3"
						},
					InterfaceConcreteTypeB = new ClassC()
					{
						TestInterfaceProperty = "Blah3", TestConcretePropertyC = "Blah3"
					}
				}
			};

			var serializer = new ConfigurationContainer().Create()
			                                             .ForTesting();

			var cycled = serializer.Cycle(outList);
			cycled.Count.Should()
			      .Be(3);
			cycled.ShouldBeEquivalentTo(outList);
			cycled.ShouldAllBeEquivalentTo(outList);
		}

		public class ClassA
		{
			public string blah { get; set; }
			public InterfaceA InterfaceConcreteTypeA { get; set; }
			public InterfaceA InterfaceConcreteTypeB { get; set; }
		}

		public class ClassB : InterfaceA
		{
			public string TestInterfaceProperty { get; set; }
			public string TestConcretePropertyB { get; set; }
		}

		public class ClassC : InterfaceA
		{
			public string TestInterfaceProperty { get; set; }
			public string TestConcretePropertyC { get; set; }
		}

		public interface InterfaceA
		{
			string TestInterfaceProperty { get; set; }
		}
	}
}