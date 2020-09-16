using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;
using FluentAssertions;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue443Tests
	{
		[Fact]
		public void Verify()
		{
			const string content = @"<Issue443Tests-TestRoot>
	<TestItems>
		<TestItem id=""TestItem1"" messageGroup=""Test"" messageNumber=""0x01"">
			<Notification>
				<StringFormat>
					Foo <Ref field=""Foo"" />
				</StringFormat>
			</Notification>
		</TestItem>
		<TestItem id=""TestItem2"" messageGroup=""Test"" messageNumber=""0x02"">
			<Command>
				<StringFormat>
					Foo <Ref field=""Foo"" />, Bar <Ref field=""Bar"" />, Baz <Ref field=""Baz"" />ms
				</StringFormat>
			</Command>

			<Response>
				<StringFormat>
					Foo <Ref field=""Foo"" />, Bar <Ref field=""Bar"" />
				</StringFormat>
			</Response>
		</TestItem>
		<TestItem id=""TestItem3"" messageGroup=""Test"" messageNumber=""0x03"">
			<Notification />
		</TestItem>

		<TestItem id=""TestItem4"" messageGroup=""Test"" messageNumber=""0x04"">
			<Notification />
		</TestItem>
	</TestItems>
</Issue443Tests-TestRoot>";

			var settings = new XmlReaderSettings
			{
				IgnoreWhitespace = false
			};
			var reader = XmlReader.Create(new StringReader(content), settings);

			var serializer = new ConfigurationContainer().EnableImplicitTyping(typeof(TestRoot))
														 /*.WithUnknownContent()
														 .Throw()*/
														 .Type<MessageStringFormat>()
														 .Register()
														 .Serializer()
														 .Using(StringFormatSerializer.Default)
														 .Create();

			serializer.Deserialize(reader).AsValid<TestRoot>().TestItems.Should().HaveCount(4);
		}

		public class TestRoot
		{
			[XmlArray]
			public List<TestItem> TestItems { get; set; }
		}

		public class TestItem
		{
			[XmlAttribute("id")]
			public string Id { get; set; }

			[XmlAttribute("messageGroup")]
			public string MessageGroup { get; set; }

			[XmlAttribute("messageNumber")]
			public string MessageNumber { get; set; }

			[XmlElement]
			public TestSubItem Notification { get; set; }

			[XmlElement]
			public TestSubItem Command { get; set; }

			[XmlElement]
			public TestSubItem Response { get; set; }
		}

		public class TestSubItem
		{
			[XmlElement]
			public MessageStringFormat StringFormat { get; set; }
		}

		public class MessageStringFormat
		{
			public string Body { get; set; }
		}

		public class StringFormatSerializer : ISerializer<MessageStringFormat>
		{
			public static StringFormatSerializer Default { get; } = new StringFormatSerializer();

			StringFormatSerializer() {}

			public MessageStringFormat Get(IFormatReader parameter)
			{
				var reader  = parameter.Get().AsValid<XmlReader>();


				var content = reader.ReadInnerXml();


				return new MessageStringFormat
				{
					Body = content
				};
			}

			public void Write(IFormatWriter writer, MessageStringFormat instance)
			{
				//throw new NotImplementedException();
			}
		}
	}
}