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
using System.Collections.Immutable;
using System.Linq;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ExtensionModel.Types
{
	public class ImmutableArrayContentsExtensionTests
	{
		[Fact]
		public void Verify()
		{
			var expected = ImmutableArray.Create("Hello", "World!");
			var container = new ConfigurationContainer();
			// container.EnableImmutableArrays();
			var serializer = new SerializationSupport(container);
			var actual = serializer.Assert(expected, @"<?xml version=""1.0"" encoding=""utf-8""?><ImmutableArray xmlns:sys=""https://extendedxmlserializer.github.io/system"" xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:arguments=""sys:string"" xmlns=""clr-namespace:System.Collections.Immutable;assembly=System.Collections.Immutable""><sys:string>Hello</sys:string><sys:string>World!</sys:string></ImmutableArray>");
			Assert.True(expected.SequenceEqual(actual));
		}
	}
}