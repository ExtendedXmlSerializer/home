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

using System;
using System.Collections.Generic;
using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ExtensionModel.Xml
{
	public class ImplicitTypingExtensionTests
	{
		[Fact]
		public void Verify()
		{
			var types = new HashSet<Type> {typeof(string), typeof(Subject), typeof(ExtendedList)};
			var serializer = new SerializationSupport(new ExtendedConfiguration().Extend(new ImplicitTypingExtension(types)));
			var expected = new Subject {Items = new List<string> {"Hello", "World!"}};

			var actual = serializer.Assert(expected,
			                               @"<?xml version=""1.0"" encoding=""utf-8""?><ImplicitTypingExtensionTests-Subject><Items><Capacity>4</Capacity><string>Hello</string><string>World!</string></Items></ImplicitTypingExtensionTests-Subject>");
			Assert.Equal(expected.Items, actual.Items);
		}

		[Fact]
		public void VerifyExplicit()
		{
			var types = new HashSet<Type> {typeof(string), typeof(Subject), typeof(ExtendedList)};
			var serializer = new SerializationSupport(new ExtendedConfiguration().Extend(new ImplicitTypingExtension(types)));
			var expected = new Subject {Items = new ExtendedList {"Hello", "World!"}};

			var actual = serializer.Assert(expected,
			                               @"<?xml version=""1.0"" encoding=""utf-8""?><ImplicitTypingExtensionTests-Subject><Items xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:type=""ImplicitTypingExtensionTests-ExtendedList""><Capacity>4</Capacity><string>Hello</string><string>World!</string></Items></ImplicitTypingExtensionTests-Subject>");
			Assert.Equal(expected.Items, actual.Items);
		}

		[Fact]
		public void VerifyConflict()
		{
			var types = new HashSet<Type> {typeof(string), typeof(Subject)};
			var configuration =
				new ExtendedConfiguration().Extend(new ImplicitTypingExtension(types))
				                           .Type<Subject>()
				                           .Name("string")
				                           .Configuration;
			Assert.Throws<InvalidOperationException>(() => configuration.Create());
		}

		class ExtendedList : List<string> {}

		class Subject
		{
			public List<string> Items { get; set; }
		}
	}
}