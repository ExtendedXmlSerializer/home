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

using ExtendedXmlSerializer.ContentModel.Properties;
using ExtendedXmlSerializer.ContentModel.Reflection;
using FluentAssertions;
using System.Collections.Immutable;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ContentModel.Properties
{
	public class TypePartsFormatterTests
	{
		[Fact]
		public void Verify()
		{
			TypePartsFormatter.Default.Get(new TypeParts("int", "testing", dimensions: ImmutableArray.Create(3, 4, 2)))
				.Should().Be("testing:int^3,4,2");

			TypePartsFormatter.Default.Get(new TypeParts("class", "testing",
					() => ImmutableArray.Create(new TypeParts("Other", "sys", dimensions: ImmutableArray.Create(1)),
						new TypeParts("Another", "native")), dimensions: ImmutableArray.Create(6, 5, 9)))
				.Should().Be("testing:class[sys:Other^1,native:Another]^6,5,9");
		}

		[Fact]
		public void VerifyTypes()
		{
			typeof(TypePartsFormatterTests).MakeArrayType().Should().Be(typeof(TypePartsFormatterTests[]));
			typeof(TypePartsFormatterTests).MakeArrayType(2).Should().Be(typeof(TypePartsFormatterTests[,]));
		}
	}
}