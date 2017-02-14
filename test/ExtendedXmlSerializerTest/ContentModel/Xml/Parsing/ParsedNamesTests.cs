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
using ExtendedXmlSerialization.ContentModel.Properties;
using ExtendedXmlSerialization.ContentModel.Xml.Parsing;
using Sprache;
using Xunit;

namespace ExtendedXmlSerialization.Test.ContentModel.Xml.Parsing
{
	public class ParsedNamesTests
	{
		const string
			Type = "SomeType",
			QualifiedType = "ns1:SomeType",
			GenericType = "ns123:GenericType[sys:string, int, ns4:SomeOtherType]",
			CompoundGenericType =
				"ns100:CompoundGenericType[sys:string, exs:AnotherGenericType[ns5:AnotherType,ns6:OneMoreType], ns40:SomeOtherType]";

		[Fact]
		public void Local()
			=> Assert.Equal(new Identity("SomeType"), ParsedNames.Default.Get(Type).Identity, IdentityEqualityComparer.Default);

		[Fact]
		public void Qualified()
		{
			var expected = new Identity("SomeType", "ns1");
			var identity = ParsedNames.Default.Get(QualifiedType).Identity;
			Assert.Equal(expected, identity,
			             IdentityEqualityComparer.Default);
		}

		[Fact]
		public void Generic()
		{
			var expected = new ParsedName(new Identity("GenericType", "ns123"), new[]
			                                                                    {
				                                                                    new ParsedName(new Identity("string", "sys")),
				                                                                    new ParsedName(new Identity("int")),
				                                                                    new ParsedName(new Identity("SomeOtherType",
				                                                                                                "ns4"))
			                                                                    }.ToImmutableArray);

			var actual = ParsedNames.Default.Get(GenericType);
			Assert.Equal(expected, actual, ParsedNameEqualityComparer.Default);
			Assert.Equal(GenericType.Replace(" ", ""), NameConverter.Default.Format(actual));
		}

		[Fact]
		public void CompoundGeneric()
		{
			var expected = new ParsedName(new Identity("CompoundGenericType", "ns100"), new[]
			                                                                           {
				                                                                           new ParsedName(new Identity("string", "sys")),
				                                                                           new ParsedName(new Identity("AnotherGenericType", "exs"),
					                                                                           new[]
					                                                                           {
						                                                                           new ParsedName(new Identity("AnotherType", "ns5")),
						                                                                           new ParsedName(new Identity("OneMoreType","ns6"))
					                                                                           }.ToImmutableArray),
				                                                                           new ParsedName(new Identity("SomeOtherType", "ns40"))
			                                                                           }.ToImmutableArray);

			var actual = ParsedNames.Default.Get(CompoundGenericType);
			Assert.Equal(CompoundGenericType.Replace(" ", ""), NameConverter.Default.Format(actual));
			Assert.Equal(expected, actual, ParsedNameEqualityComparer.Default);
		}

		[Fact]
		public void List()
		{
			var actual = TypesList.Default.Get("sys:string, int, ns4:SomeOtherType");
			Assert.True(new[]
			             {
				             new ParsedName(new Identity("string", "sys")),
				             new ParsedName(new Identity("int")),
				             new ParsedName(new Identity("SomeOtherType", "ns4"))
			             }.ToImmutableArray().SequenceEqual(actual, ParsedNameEqualityComparer.Default));
		}

		[Fact]
		public void InvalidList()
		{
			var actual = TypesList.Default.Get().TryParse("[sys:string, int, ns4:SomeOtherType]").WasSuccessful;
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

		sealed class IdentityEqualityComparer : IEqualityComparer<Identity>
		{
			public static IdentityEqualityComparer Default { get; } = new IdentityEqualityComparer();
			IdentityEqualityComparer() {}

			public bool Equals(Identity x, Identity y)
				=> string.Equals(x.Name, y.Name) && string.Equals(x.Identifier, y.Identifier);

			public int GetHashCode(Identity obj)
			{
				unchecked
				{
					return ((obj.Name?.GetHashCode() ?? 0) * 397) ^
					       (obj.Identifier?.GetHashCode() ?? 0);
				}
			}
		}

		sealed class ParsedNameEqualityComparer : IEqualityComparer<ParsedName>
		{
			public static ParsedNameEqualityComparer Default { get; } = new ParsedNameEqualityComparer();
			ParsedNameEqualityComparer() {}

			public bool Equals(ParsedName x, ParsedName y)
			{
				var argumentsX = x.GetArguments().GetValueOrDefault(ImmutableArray<ParsedName>.Empty);
				var argumentsY = y.GetArguments().GetValueOrDefault(ImmutableArray<ParsedName>.Empty);

				var arguments = argumentsX.SequenceEqual(argumentsY, this);
				var identity = IdentityEqualityComparer.Default.Equals(x.Identity, y.Identity);
				var result = arguments && identity;
				return result;
			}

			public int GetHashCode(ParsedName obj)
			{
				unchecked
				{
					return ((obj.GetArguments()?.GetHashCode() ?? 0) * 397) ^
					       IdentityEqualityComparer.Default.GetHashCode(obj.Identity);
				}
			}
		}
	}
}