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
using System.Collections.ObjectModel;
using System.Reflection;
using ExtendedXmlSerialization.Configuration;
using ExtendedXmlSerialization.ExtensionModel;
using ExtendedXmlSerialization.Test.Support;
using JetBrains.Annotations;
using Xunit;

namespace ExtendedXmlSerialization.Test.ExtensionModel
{
	public class EncryptionExtensionTests
	{
		[Fact]
		public void SimpleString()
		{
			const string message = "Hello World!  This is my encrypted message!";

			var typeInfo = typeof(SimpleSubject).GetTypeInfo();

			var converters = new Collection<MemberInfo>
			                 {
								 typeInfo.GetProperty(nameof(SimpleSubject.Message))
							 };
			var sut = new EncryptionExtension(converters);

			var extensions = DefaultExtensions.Default.With(sut, EmitBehaviorExtension.Default);
			var support = new SerializationSupport(new ExtendedXmlConfiguration(extensions).Create());
			var expected = new SimpleSubject {Message = message};
			var actual = support.Assert(expected,
			                            @"<?xml version=""1.0"" encoding=""utf-8""?><EncryptionExtensionTests-SimpleSubject Message=""SGVsbG8gV29ybGQhICBUaGlzIGlzIG15IGVuY3J5cHRlZCBtZXNzYWdlIQ=="" xmlns=""clr-namespace:ExtendedXmlSerialization.Test.ExtensionModel;assembly=ExtendedXmlSerializerTest"" />");
			Assert.Equal(message, actual.Message);
		}

		[Fact]
		public void SimpleNonString()
		{
			const string message = "Hello World!  This is my unencrypted message!";
			var identifier = new Guid("B496F7F5-58F8-41BF-AF18-117B8F3743BF");

			var typeInfo = typeof(SimpleSubject).GetTypeInfo();

			var converters = new Collection<MemberInfo>
			                 {
								 typeInfo.GetProperty(nameof(SimpleSubject.Identifier))
							 };

			var sut = new EncryptionExtension(converters);

			var support = new SerializationSupport(new ExtendedXmlConfiguration().Extend(sut).Create());
			var expected = new SimpleSubject {Identifier = identifier, Message = message};
			var actual = support.Assert(expected,
			                            @"<?xml version=""1.0"" encoding=""utf-8""?><EncryptionExtensionTests-SimpleSubject Identifier=""YjQ5NmY3ZjUtNThmOC00MWJmLWFmMTgtMTE3YjhmMzc0M2Jm"" xmlns=""clr-namespace:ExtendedXmlSerialization.Test.ExtensionModel;assembly=ExtendedXmlSerializerTest""><Message>Hello World!  This is my unencrypted message!</Message></EncryptionExtensionTests-SimpleSubject>");
			Assert.Equal(identifier, actual.Identifier);
			Assert.Equal(message, actual.Message);
		}

		class SimpleSubject
		{
			[UsedImplicitly]
			public Guid Identifier { get; set; }

			[UsedImplicitly]
			public string Message { get; set; }
		}
	}
}