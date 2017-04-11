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
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ReflectionModel;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReflectionModel
{
	public class TypeDefinitionIdentityComparerTests
	{
		[Fact]
		public void Verify()
		{
			var expected = typeof(IDictionary<,>).GetTypeInfo();
			var actual = typeof(Dictionary<,>).GetInterfaces().First().GetTypeInfo();
			Assert.NotStrictEqual(expected, actual);
			Assert.NotSame(expected, actual);

			var sut = TypeDefinitionIdentityComparer.Default;

			Assert.Equal(expected, actual, sut);
			var info = typeof(IDictionary<object, object>).GetTypeInfo();

			Assert.NotStrictEqual(expected, info);
			Assert.Equal(expected, info, sut);

			Assert.False(typeof(IDictionary<,>).IsAssignableFrom(typeof(Dictionary<,>)));
			Assert.True(typeof(IDictionary<string, object>).IsAssignableFrom(typeof(Dictionary<string, object>)));
		}
	}
}