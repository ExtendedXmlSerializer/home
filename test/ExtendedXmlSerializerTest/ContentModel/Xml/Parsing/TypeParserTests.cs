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

		sealed class QualifiedNameEqualityComparer : IEqualityComparer<QualifiedName>
		{
			public static QualifiedNameEqualityComparer Default { get; } = new QualifiedNameEqualityComparer();

			QualifiedNameEqualityComparer() : this(QualifiedNameFormatter.Default) {}

			readonly IQualifiedNameFormatter _formatter;
			
			QualifiedNameEqualityComparer(IQualifiedNameFormatter formatter)
			{
				_formatter = formatter;
			}

			public bool Equals(QualifiedName x, QualifiedName y) => string.Equals(_formatter.Get(x), _formatter.Get(y));

			public int GetHashCode(QualifiedName obj) => _formatter.Get(obj).GetHashCode();
		}

		[Fact]
		public void Local() => Assert.Equal(new QualifiedName("SomeType"), QualifiedNameParser.Default.Get(Type), QualifiedNameEqualityComparer.Default);

		[Fact]
		public void Qualified() => Assert.Equal(new QualifiedName("SomeType", "ns1"), QualifiedNameParser.Default.Get(QualifiedType), QualifiedNameEqualityComparer.Default);

		[Fact]
		public void Generic()
		{
			var expected = new QualifiedName("GenericType", "ns123", new[]
			             {
				             new QualifiedName("string", "sys"),
				             new QualifiedName("int"),
				             new QualifiedName("SomeOtherType", "ns4")
			             }.ToImmutableArray);

			var actual = QualifiedNameParser.Default.Get(GenericType);
			Assert.Equal(expected, actual, QualifiedNameEqualityComparer.Default);
			Assert.Equal(GenericType.Replace(" ", ""), actual.ToString());
		}

		[Fact]
		public void CompoundGeneric()
		{
			var expected = new QualifiedName("CompoundGenericType", "ns100", new[]
							{
								new QualifiedName("string", "sys"),
								new QualifiedName("AnotherGenericType", "exs",
								new[]
								{
									new QualifiedName("AnotherType", "ns5"),
									new QualifiedName("OneMoreType", "ns6")
								}.ToImmutableArray),
								new QualifiedName("SomeOtherType", "ns40")
							}.ToImmutableArray);

			var actual = QualifiedNameParser.Default.Get(CompoundGenericType);
			Assert.Equal(expected, actual, QualifiedNameEqualityComparer.Default);
			Assert.Equal(CompoundGenericType.Replace(" ", ""), actual.ToString());
		}

		[Fact]
		public void List()
		{
			var actual = QualifiedNameListParser.Default.Get().Parse("sys:string, int, ns4:SomeOtherType");
			Assert.True(new[]
			             {
				             new QualifiedName("string", "sys"),
				             new QualifiedName("int"),
				             new QualifiedName("SomeOtherType", "ns4")
			             }.ToImmutableArray().SequenceEqual(actual, QualifiedNameEqualityComparer.Default));
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