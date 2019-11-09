using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ContentModel.Members
{
	public class AttributeSpecificationTests
	{
		[Fact]
		public void Verify()
		{
			const string target = "I am Attribute, Hear me roar! #rawr!";

			var configuration = new ConfigurationContainer();
			configuration.Type<SimpleSubject>()
			             .Member(x => x.Message)
			             .Attribute(x => x == target);

			var support  = new SerializationSupport(configuration);
			var expected = new SimpleSubject {Message = "Hello World!", Number = 6776};
			var content = support.Assert(expected,
			                             @"<?xml version=""1.0"" encoding=""utf-8""?><AttributeSpecificationTests-SimpleSubject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ContentModel.Members;assembly=ExtendedXmlSerializer.Tests""><Message>Hello World!</Message><Number>6776</Number></AttributeSpecificationTests-SimpleSubject>");
			Assert.Equal(expected.Message, content.Message);
			Assert.Equal(expected.Number, content.Number);

			expected.Message = target;
			var attributes = support.Assert(expected,
			                                @"<?xml version=""1.0"" encoding=""utf-8""?><AttributeSpecificationTests-SimpleSubject Message=""I am Attribute, Hear me roar! #rawr!"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ContentModel.Members;assembly=ExtendedXmlSerializer.Tests""><Number>6776</Number></AttributeSpecificationTests-SimpleSubject>");
			Assert.Equal(expected.Message, attributes.Message);
			Assert.Equal(expected.Number, attributes.Number);
		}

		class SimpleSubject
		{
			public string Message { get; set; }

			public int Number { get; set; }
		}
	}
}