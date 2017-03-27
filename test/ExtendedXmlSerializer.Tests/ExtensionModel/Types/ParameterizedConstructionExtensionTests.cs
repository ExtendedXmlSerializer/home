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
using ExtendedXmlSerializer.Tests.Support;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ExtensionModel.Types
{
	public class ParameterizedConstructionExtensionTests
	{
		[Fact]
		public void Verify()
		{
			var serializer = new SerializationSupport(new ExtendedConfiguration().Extend(ParameterizedConstructionExtension.Default));
			var expected = new Subject("Hello World!");
			var actual = serializer.Assert(expected, @"<?xml version=""1.0"" encoding=""utf-8""?><ParameterizedConstructionExtensionTests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Types;assembly=ExtendedXmlSerializer.Tests""><Message>Hello World!</Message></ParameterizedConstructionExtensionTests-Subject>");
			Assert.Equal(expected.Message, actual.Message);
		}

		class Subject
		{
			public Subject(string message)
			{
				Message = message;
			}

			public string Message { get; }
		}
	}
}