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
			var myMessage = new MyMessage
			{
				MyElement = new MyElementType
				{
					UniqueId = Guid.NewGuid()
					               .ToString()
				}
			};
			new ConfigurationContainer().ForTesting()
			                            .Cycle(myMessage)
			                            .ShouldBeEquivalentTo(myMessage);
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
}