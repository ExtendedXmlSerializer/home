// MIT License
//
// Copyright (c) 2016 Wojciech Nagórski
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
using ExtendedXmlSerializer.ExtensionModel.Types;
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

			var configuration = new ExtendedConfiguration()
				.Type<SimpleSubject>()
				.Member(x => x.Message)
				.Attribute(x => x == target)
				.Configuration;

			var support = new SerializationSupport(configuration);
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