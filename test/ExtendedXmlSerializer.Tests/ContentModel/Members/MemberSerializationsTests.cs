using ExtendedXmlSerializer.Tests.Support;
using ExtendedXmlSerializer.Tests.TestObject;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ContentModel.Members
{
	public class MemberSerializationsTests
	{
		[Fact]
		public void XmlElementAttribute()
		{
			var expected = new TestClassWithXmlElementAttribute {Id = 123};
			var actual = new SerializationSupport().Assert(
			                                               expected,
			                                               @"<?xml version=""1.0"" encoding=""utf-8""?><TestClassWithXmlElementAttribute xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests""><Identifier>123</Identifier></TestClassWithXmlElementAttribute>"
			                                              );
			Assert.Equal(expected.Id, actual.Id);
		}

		[Fact]
		public void XmlElementWithOrder()
		{
			var expected = new TestClassWithOrderParameters {A = "A", B = "B"};
			var actual = new SerializationSupport().Assert(
			                                               expected,
			                                               @"<?xml version=""1.0"" encoding=""utf-8""?><TestClassWithOrderParameters xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests""><A>A</A><B>B</B></TestClassWithOrderParameters>"
			                                              );
			Assert.Equal(expected.A, actual.A);
			Assert.Equal(expected.B, actual.B);
		}

		[Fact]
		public void XmlElementWithOrderExt()
		{
			var expected = new TestClassWithOrderParametersExt {A = "A", B = "B", C = "C", D = "D"};
			var actual = new SerializationSupport().Assert(
			                                               expected,
			                                               @"<?xml version=""1.0"" encoding=""utf-8""?><TestClassWithOrderParametersExt xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests""><A>A</A><B>B</B><D>D</D><C>C</C></TestClassWithOrderParametersExt>"
			                                              );
			Assert.Equal(expected.A, actual.A);
			Assert.Equal(expected.B, actual.B);
			Assert.Equal(expected.C, actual.C);
			Assert.Equal(expected.D, actual.D);
		}

		[Fact]
		public void TestClassInheritanceWithDefaultOrder()
		{
			TestClassInheritanceBase expected = new TestClassInheritance();
			expected.Init();
			var actual =
				new SerializationSupport().Assert(expected,
				                                  @"<?xml version=""1.0"" encoding=""utf-8""?><TestClassInheritance xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests""><Id>2</Id><Id2>3</Id2></TestClassInheritance>");
			actual.Should().BeEquivalentTo(expected);
		}

		[Fact]
		public void TestClassInheritanceWithOrder()
		{
			var expected = TestObject.TestClassInheritanceWithOrder.Create();
			var actual = new SerializationSupport().Assert(
			                                               expected,
			                                               @"<?xml version=""1.0"" encoding=""utf-8""?><TestClassInheritanceWithOrder xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests""><Id2>3</Id2><Id>2</Id></TestClassInheritanceWithOrder>"
			                                              );
			Assert.Equal(expected.Id2, actual.Id2);
			Assert.Equal(expected.Id, actual.Id);
		}

		[Fact]
		public void TestClassInterfaceInheritanceWithOrder()
		{
			var expected = new TestClassInterfaceInheritanceWithOrder {Id = 1, Id2 = 2};
			var actual = new SerializationSupport().Assert(
			                                               expected,
			                                               @"<?xml version=""1.0"" encoding=""utf-8""?><TestClassInterfaceInheritanceWithOrder xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests""><Id2>2</Id2><Id>1</Id></TestClassInterfaceInheritanceWithOrder>"
			                                              );
			Assert.Equal(expected.Id2, actual.Id2);
			Assert.Equal(expected.Id, actual.Id);
		}

		[Fact]
		public void XmlRoot()
		{
			var expected = new TestClassWithXmlRootAttribute {Id = 123};
			var actual = new SerializationSupport().Assert(
			                                               expected,
			                                               @"<?xml version=""1.0"" encoding=""utf-8""?><TestClass xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests""><Id>123</Id></TestClass>"
			                                              );
			Assert.Equal(expected.Id, actual.Id);
		}
	}
}