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

using ExtendedXmlSerialization.Configuration;
using ExtendedXmlSerialization.ExtensionModel;
using ExtendedXmlSerialization.Test.Support;
using JetBrains.Annotations;
using Xunit;

namespace ExtendedXmlSerialization.Test.ExtensionModel
{
	public class ReferencesExtensionTests
	{
		[Fact]
		public void Verify()
		{
			var support = new SerializationSupport(new ExtendedXmlConfiguration().Extend(new ReferencesExtension()).Create());
			var instance = new Subject {PropertyName = "Hello World!"};
			instance.Self = instance;
			var actual = support.Assert(instance,
			               @"<?xml version=""1.0"" encoding=""utf-8""?><ReferencesExtensionTests-Subject xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:identity=""1"" xmlns=""clr-namespace:ExtendedXmlSerialization.Test.ExtensionModel;assembly=ExtendedXmlSerializerTest""><Self exs:reference=""1"" /><PropertyName>Hello World!</PropertyName></ReferencesExtensionTests-Subject>");
			Assert.NotNull(actual.Self);
			Assert.Same(actual, actual.Self);
		}

		class Subject
		{
			[UsedImplicitly]
			public Subject Self { get; set; }

			[UsedImplicitly]
			public string PropertyName { get; set; }
		}
	}
}