// MIT License
//
// Copyright (c) 2016-2018 Wojciech Nagórski
//                    Michael DeMond
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System;
using System.Xml.Serialization;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public class Issue175Tests
	{
		[Fact]
		public void Verify()
		{
			var element = new MyElementType
			{
				UniqueId = "Message"
			};
			var myMessage = new MyMessage
			{
				MyElement = element
			};
			var support = new ConfigurationContainer().InspectingType<MyMessage>().ForTesting();
			support.Assert(myMessage, @"<?xml version=""1.0"" encoding=""utf-8""?><myMessage xmlns=""http://namespace/file.xsd""><myElement uniqueId=""Message"" /></myMessage>");
			support.Cycle(myMessage)
				   .ShouldBeEquivalentTo(myMessage);

			support.Cycle(element).ShouldBeEquivalentTo(element);
		}

		[Fact]
		public void None()
		{
			var support = new ConfigurationContainer().InspectingType<None>().ForTesting();
			support.Assert(new None{ UniqueId = "123"}, @"<?xml version=""1.0"" encoding=""utf-8""?><None uniqueId=""123"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests"" />");

		}

		[Fact]
		public void VerifyRead()
		{
			var xml = @"<?xml version=""1.0""?>
						<myMessage xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://namespace/file.xsd"">
							<myElement uniqueId=""12345"" />
						</myMessage>";

			new ConfigurationContainer().InspectingType<MyMessage>().ForTesting()
										.Deserialize<MyMessage>(xml)
										.MyElement.UniqueId.Should()
										.Be("12345");


		}
	}

	[Serializable]
	[XmlType(AnonymousType          = true, Namespace                         = "http://namespace/file.xsd")]
	[XmlRoot("myMessage", Namespace = "http://namespace/file.xsd", IsNullable = false)]
	public class MyMessage
	{
		/// <remarks />
		[XmlElement("myElement")]
		public MyElementType MyElement { get; set; }
	}

	/// <remarks />
	[XmlType(Namespace = "http://namespace/file.xsd")]
	public class MyElementType
	{
		/// <remarks />
		[XmlAttribute("uniqueId")]
		public string UniqueId { get; set; }
	}

	[XmlType(Namespace = "")]
	public class None
	{
		/// <remarks />
		[XmlAttribute("uniqueId")]
		public string UniqueId { get; set; }
	}
}