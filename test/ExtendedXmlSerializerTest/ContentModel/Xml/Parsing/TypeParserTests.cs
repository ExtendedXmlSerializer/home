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
using System.Collections.Immutable;
using System.Linq;
using System.Xml;
using ExtendedXmlSerialization.ContentModel.Xml.Parsing;
using Sprache;
using Xunit;

namespace ExtendedXmlSerialization.Test.ContentModel.Xml.Parsing
{
	public class TypeParserTests
	{
		const string
			Type = "SomeType",
			QualifiedType = "ns1:SomeType",
			GenericType = "ns123:GenericType[sys:string, int, ns4:SomeOtherType]",
			CompoundGenericType =
				"ns100:CompoundGenericType[sys:string, exs:AnotherGenericType[ns5:AnotherType,ns6:OneMoreType], ns40:SomeOtherType]";

		[Fact]
		public void Local() => Assert.Equal(new XmlQualifiedName("SomeType"), NameParser.Default.Get(Type));

		[Fact]
		public void Qualified() => Assert.Equal(new XmlQualifiedName("SomeType", "ns1"), NameParser.Default.Get(QualifiedType));

		[Fact]
		public void Generic()
		{
			var expected = new GenericXmlQualifiedName("GenericType", "ns123", new[]
			             {
				             new XmlQualifiedName("string", "sys"),
				             new XmlQualifiedName("int"),
				             new XmlQualifiedName("SomeOtherType", "ns4")
			             }.ToImmutableArray());

			var actual = NameParser.Default.Get(GenericType);
			Assert.Equal(expected, actual);
			Assert.Equal(GenericType.Replace(" ", ""), actual.ToString());
		}

		[Fact]
		public void CompoundGeneric()
		{
			var expected = new GenericXmlQualifiedName("CompoundGenericType", "ns100", new[]
							{
								new XmlQualifiedName("string", "sys"),
								new GenericXmlQualifiedName("AnotherGenericType", "exs",
								new[]
								{
									new XmlQualifiedName("AnotherType", "ns5"),
									new XmlQualifiedName("OneMoreType", "ns6")
								}.ToImmutableArray()),
								new XmlQualifiedName("SomeOtherType", "ns40")
							}.ToImmutableArray());

			var actual = NameParser.Default.Get(CompoundGenericType);
			Assert.Equal(expected, actual);
			Assert.Equal(CompoundGenericType.Replace(" ", ""), actual.ToString());
		}

		[Fact]
		public void List()
		{
			var actual = QualifiedNameListParser.Default.Get().Parse("sys:string, int, ns4:SomeOtherType");
			Assert.True(new[]
			             {
				             new XmlQualifiedName("string", "sys"),
				             new XmlQualifiedName("int"),
				             new XmlQualifiedName("SomeOtherType", "ns4")
			             }.ToImmutableArray().SequenceEqual(actual));
		}

		[Fact]
		public void InvalidList()
		{
			var actual = QualifiedNameListParser.Default.Get().TryParse("[sys:string, int, ns4:SomeOtherType]").WasSuccessful;
			Assert.False(actual);
		}

		[Fact]
		public void AssemblyPathParser()
		{
			const string input = "clr-namespace:ExtendedXmlSerialization.Test.TestObject;assembly=ExtendedXmlSerializerTest";
			var path = ExtendedXmlSerialization.ContentModel.Xml.AssemblyPathParser.Default.Get(input);
			Assert.Equal("ExtendedXmlSerialization.Test.TestObject", path.Namespace);
			Assert.Equal("ExtendedXmlSerializerTest", path.Path);
		}

		[Fact]
		public void GenericName()
		{
			var name = typeof(Dictionary<,>).Name;
			var result = GenericNameParser.Default.Get(name);
			Assert.Equal("Dictionary", result);
		}
	}
}