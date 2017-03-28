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
using ExtendedXmlSerializer.ContentModel.Properties;
using ExtendedXmlSerializer.ContentModel.Xml;
using ExtendedXmlSerializer.ContentModel.Xml.Parsing;
using ExtendedXmlSerializer.Core.Sprache;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ContentModel.Xml.Parsing
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
			=> Assert.Equal(new TypeParts("SomeType"), ParsedNames.Default.Get(Type), ParsedNameEqualityComparer.Default);

		[Fact]
		public void Qualified()
		{
			var expected = new TypeParts("SomeType", "ns1");
			var identity = ParsedNames.Default.Get(QualifiedType);
			Assert.Equal(expected, identity, ParsedNameEqualityComparer.Default);
		}

		[Fact]
		public void Generic()
		{
			var expected = new TypeParts("GenericType", "ns123", new[]
			                                                      {
				                                                      new TypeParts("string", "sys"),
				                                                      new TypeParts("int"),
				                                                      new TypeParts("SomeOtherType", "ns4")
			                                                      }.ToImmutableArray);

			var actual = ParsedNames.Default.Get(GenericType);
			Assert.Equal(expected, actual, ParsedNameEqualityComparer.Default);
			Assert.Equal(GenericType.Replace(" ", ""), NameConverter.Default.Format(actual));
		}

		[Fact]
		public void CompoundGeneric()
		{
			var expected = new TypeParts("CompoundGenericType", "ns100", new[]
			                                                              {
				                                                              new TypeParts("string", "sys"),
				                                                              new TypeParts("AnotherGenericType", "exs",
				                                                                             new[]
				                                                                             {
					                                                                             new TypeParts("AnotherType", "ns5"),
					                                                                             new TypeParts("OneMoreType", "ns6")
				                                                                             }.ToImmutableArray),
				                                                              new TypeParts("SomeOtherType", "ns40")
			                                                              }.ToImmutableArray);

			var actual = ParsedNames.Default.Get(CompoundGenericType);
			Assert.Equal(CompoundGenericType.Replace(" ", ""), NameConverter.Default.Format(actual));
			Assert.Equal(expected, actual, ParsedNameEqualityComparer.Default);
		}

		[Fact]
		public void List()
		{
			var actual = TypePartsList.Default.Get("sys:string, int, ns4:SomeOtherType");
			Assert.True(new[]
			            {
				            new TypeParts("string", "sys"),
				            new TypeParts("int"),
				            new TypeParts("SomeOtherType", "ns4")
			            }.ToImmutableArray().SequenceEqual(actual, ParsedNameEqualityComparer.Default));
		}

		[Fact]
		public void InvalidList()
		{
			var actual = TypePartsList.Default.Get().TryParse("[sys:string, int, ns4:SomeOtherType]").WasSuccessful;
			Assert.False(actual);
		}

		[Fact]
		public void AssemblyPathParser()
		{
			const string input = "clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests";
			var path = global::ExtendedXmlSerializer.ContentModel.Xml.AssemblyPathParser.Default.Get(input);
			Assert.Equal("ExtendedXmlSerializer.Tests.TestObject", path.Namespace);
			Assert.Equal("ExtendedXmlSerializer.Tests", path.Path);
		}

		[Fact]
		public void GenericName()
		{
			var name = typeof(Dictionary<,>).Name;
			var result = GenericNameParser.Default.Get(name);
			Assert.Equal("Dictionary", result);
		}

		sealed class ParsedNameEqualityComparer : IEqualityComparer<TypeParts>
		{
			readonly static IdentityComparer<TypeParts> Comparer = IdentityComparer<TypeParts>.Default;

			public static ParsedNameEqualityComparer Default { get; } = new ParsedNameEqualityComparer();
			ParsedNameEqualityComparer() {}

			public bool Equals(TypeParts x, TypeParts y)
			{
				var argumentsX = x.GetArguments().GetValueOrDefault(ImmutableArray<TypeParts>.Empty);
				var argumentsY = y.GetArguments().GetValueOrDefault(ImmutableArray<TypeParts>.Empty);

				var arguments = argumentsX.SequenceEqual(argumentsY, this);
				var identity = Comparer.Equals(x, y);
				var result = arguments && identity;
				return result;
			}

			public int GetHashCode(TypeParts obj)
			{
				unchecked
				{
					return ((obj.GetArguments()?.GetHashCode() ?? 0) * 397) ^ Comparer.GetHashCode(obj);
				}
			}
		}
	}
}