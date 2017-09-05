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
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Encryption;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using JetBrains.Annotations;
using System;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ExtensionModel.Encryption
{
	public class EncryptionExtensionTests
	{
		[Fact]
		public void SimpleString()
		{
			const string message = "Hello World!  This is my encrypted message!";
			var support = new SerializationSupport(new ConfigurationContainer().Emit(EmitBehaviors.Assigned)
			                                                                   .Type<SimpleSubject>()
			                                                                   .Member(x => x.Message)
			                                                                   .Encrypt()
			                                                                   .Create());
			var expected = new SimpleSubject {Message = message};
			var actual = support.Assert(expected,
										@"<?xml version=""1.0"" encoding=""utf-8""?><EncryptionExtensionTests-SimpleSubject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Encryption;assembly=ExtendedXmlSerializer.Tests""><Message>SGVsbG8gV29ybGQhICBUaGlzIGlzIG15IGVuY3J5cHRlZCBtZXNzYWdlIQ==</Message></EncryptionExtensionTests-SimpleSubject>");
			Assert.Equal(message, actual.Message);
		}

		[Fact]
		public void SimpleStringAsAttribute()
		{
			const string message = "Hello World!  This is my encrypted message!";
			var support = new SerializationSupport(new ConfigurationContainer().Emit(EmitBehaviors.Assigned)
			                                                                   .Type<SimpleSubject>()
			                                                                   .Member(x => x.Message).Attribute().Encrypt()
																			   .Create());
			var expected = new SimpleSubject { Message = message };
			var actual = support.Assert(expected,
										@"<?xml version=""1.0"" encoding=""utf-8""?><EncryptionExtensionTests-SimpleSubject Message=""SGVsbG8gV29ybGQhICBUaGlzIGlzIG15IGVuY3J5cHRlZCBtZXNzYWdlIQ=="" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Encryption;assembly=ExtendedXmlSerializer.Tests"" />");
			Assert.Equal(message, actual.Message);
		}

		[Fact]
		public void SimpleNonString()
		{
			const string message = "Hello World!  This is my unencrypted message!";
			var identifier = new Guid("B496F7F5-58F8-41BF-AF18-117B8F3743BF");

			var support = new SerializationSupport(new ConfigurationContainer().Type<SimpleSubject>().Member(x => x.Identifier).Encrypt().Create());
			var expected = new SimpleSubject {Identifier = identifier, Message = message};
			var actual = support.Assert(expected,
										@"<?xml version=""1.0"" encoding=""utf-8""?><EncryptionExtensionTests-SimpleSubject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Encryption;assembly=ExtendedXmlSerializer.Tests""><Identifier>YjQ5NmY3ZjUtNThmOC00MWJmLWFmMTgtMTE3YjhmMzc0M2Jm</Identifier><Message>Hello World!  This is my unencrypted message!</Message></EncryptionExtensionTests-SimpleSubject>");
			Assert.Equal(identifier, actual.Identifier);
			Assert.Equal(message, actual.Message);
		}

		[Fact]
		public void SimpleNonStringAsAttribute()
		{
			const string message = "Hello World!  This is my unencrypted message!";
			var identifier = new Guid("B496F7F5-58F8-41BF-AF18-117B8F3743BF");

			var support = new SerializationSupport(new ConfigurationContainer().Type<SimpleSubject>().Member(x => x.Identifier).Attribute().Encrypt().Create());
			var expected = new SimpleSubject { Identifier = identifier, Message = message };
			var actual = support.Assert(expected,
										@"<?xml version=""1.0"" encoding=""utf-8""?><EncryptionExtensionTests-SimpleSubject Identifier=""YjQ5NmY3ZjUtNThmOC00MWJmLWFmMTgtMTE3YjhmMzc0M2Jm"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Encryption;assembly=ExtendedXmlSerializer.Tests""><Message>Hello World!  This is my unencrypted message!</Message></EncryptionExtensionTests-SimpleSubject>");
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