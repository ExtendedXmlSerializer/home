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

using System.Collections.Generic;
using ExtendedXmlSerializer.ContentModel.Xml.Parsing;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Markup;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ExtensionModel.Markup
{
	public class MarkupExtensionParserTests
	{
		[Fact]
		public void Type()
		{
			const string text = "{x:Testing}";
			var parts = MarkupExtensionParser.Default.Get(text);
			parts.Type.ShouldBeEquivalentTo(new TypeParts("Testing", "x"));
			parts.Arguments.Should().BeEmpty();
		}

		[Fact]
		public void VerifySpaced()
		{
			const string text = "{x:Testing }";
			var parts = MarkupExtensionParser.Default.Get(text);
			parts.Type.ShouldBeEquivalentTo(new TypeParts("Testing", "x"));
			parts.Arguments.Should().BeEmpty();
		}

		[Fact]
		public void VerifyArguments()
		{
			const string text = "{x:Testing 'one', 12345, ' two ', '{}This is an escaped {} literal.', 3 * 3}";
			var parts = MarkupExtensionParser.Default.Get(text);
			parts.Type.ShouldBeEquivalentTo(new TypeParts("Testing", "x"));
			parts.Arguments
			     .Should().HaveCount(5)
			     .And
			     .ContainInOrder("one".Quoted(), "12345", " two ".Quoted(), "This is an escaped {} literal.".Quoted(), "3 * 3");
			parts.Properties.Should().BeEmpty();
		}

		[Fact]
		public void VerifyProperties()
		{
			const string text = "{x:Testing MemberName='Value'}";
			var parts = MarkupExtensionParser.Default.Get(text);
			parts.Type.ShouldBeEquivalentTo(new TypeParts("Testing", "x"));
			parts.Arguments.Should().BeEmpty();
			parts.Properties.ShouldBeEquivalentTo(new Dictionary<string, string> {{"MemberName", "Value".Quoted()}});
		}

		[Fact]
		public void VerifyArgumentsAndProperties()
		{
			const string text = "{x:Testing 'one', 12345, ' two ', MemberName='Value', MemberTwo = 3 + 4}";
			var parts = MarkupExtensionParser.Default.Get(text);
			parts.Type.ShouldBeEquivalentTo(new TypeParts("Testing", "x"));
			parts.Arguments.Should().HaveCount(3).And.BeEquivalentTo("one".Quoted(), "12345", " two ".Quoted());
			parts.Properties.ShouldBeEquivalentTo(new Dictionary<string, string>
			                                      {
				                                      {"MemberName", "Value".Quoted()},
				                                      {"MemberTwo", "3 + 4"}
			                                      });
		}
	}
}