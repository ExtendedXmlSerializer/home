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
			                                                                   .Member(x => x.Message)
			                                                                   .Attribute()
			                                                                   .Encrypt()
			                                                                   .Create());
			var expected = new SimpleSubject {Message = message};
			var actual = support.Assert(expected,
			                            @"<?xml version=""1.0"" encoding=""utf-8""?><EncryptionExtensionTests-SimpleSubject Message=""SGVsbG8gV29ybGQhICBUaGlzIGlzIG15IGVuY3J5cHRlZCBtZXNzYWdlIQ=="" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Encryption;assembly=ExtendedXmlSerializer.Tests"" />");
			Assert.Equal(message, actual.Message);
		}

		[Fact]
		public void SimpleNonString()
		{
			const string message    = "Hello World!  This is my unencrypted message!";
			var          identifier = new Guid("B496F7F5-58F8-41BF-AF18-117B8F3743BF");

			var support = new SerializationSupport(new ConfigurationContainer().Type<SimpleSubject>()
			                                                                   .Member(x => x.Identifier)
			                                                                   .Encrypt()
			                                                                   .Create());
			var expected = new SimpleSubject {Identifier = identifier, Message = message};
			var actual = support.Assert(expected,
			                            @"<?xml version=""1.0"" encoding=""utf-8""?><EncryptionExtensionTests-SimpleSubject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Encryption;assembly=ExtendedXmlSerializer.Tests""><Identifier>YjQ5NmY3ZjUtNThmOC00MWJmLWFmMTgtMTE3YjhmMzc0M2Jm</Identifier><Message>Hello World!  This is my unencrypted message!</Message></EncryptionExtensionTests-SimpleSubject>");
			Assert.Equal(identifier, actual.Identifier);
			Assert.Equal(message, actual.Message);
		}

		[Fact]
		public void SimpleNonStringAsAttribute()
		{
			const string message    = "Hello World!  This is my unencrypted message!";
			var          identifier = new Guid("B496F7F5-58F8-41BF-AF18-117B8F3743BF");

			var support = new SerializationSupport(new ConfigurationContainer().Type<SimpleSubject>()
			                                                                   .Member(x => x.Identifier)
			                                                                   .Attribute()
			                                                                   .Encrypt()
			                                                                   .Create());
			var expected = new SimpleSubject {Identifier = identifier, Message = message};
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