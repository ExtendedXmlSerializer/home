using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using System;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ExtensionModel.Xml
{
	public class ImplicitTypingExtensionTests
	{
		[Fact]
		public void Verify()
		{
			var types      = new HashSet<Type> {typeof(string), typeof(Subject), typeof(ExtendedList)};
			var serializer = new SerializationSupport(new ConfigurationContainer().EnableImplicitTyping(types));
			var expected   = new Subject {Items = new List<string> {"Hello", "World!"}};

			var actual = serializer.Assert(expected,
			                               @"<?xml version=""1.0"" encoding=""utf-8""?><ImplicitTypingExtensionTests-Subject><Items><Capacity>4</Capacity><string>Hello</string><string>World!</string></Items></ImplicitTypingExtensionTests-Subject>");
			Assert.Equal(expected.Items, actual.Items);
		}

		[Fact]
		public void VerifyExplicit()
		{
			var types      = new HashSet<Type> {typeof(string), typeof(Subject), typeof(ExtendedList)};
			var serializer = new SerializationSupport(new ConfigurationContainer().EnableImplicitTyping(types));
			var expected   = new Subject {Items = new ExtendedList {"Hello", "World!"}};

			var actual = serializer.Assert(expected,
			                               @"<?xml version=""1.0"" encoding=""utf-8""?><ImplicitTypingExtensionTests-Subject><Items xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:type=""ImplicitTypingExtensionTests-ExtendedList""><Capacity>4</Capacity><string>Hello</string><string>World!</string></Items></ImplicitTypingExtensionTests-Subject>");
			Assert.Equal(expected.Items, actual.Items);
		}

		[Fact]
		public void VerifyConflict()
		{
			var types         = new HashSet<Type> {typeof(string), typeof(Subject)};
			var configuration = new ConfigurationContainer();
			configuration.EnableImplicitTyping(types)
			             .Type<Subject>()
			             .Name("string");
			Assert.Throws<InvalidOperationException>(() => configuration.Create());
		}

		class ExtendedList : List<string> {}

		class Subject
		{
			public List<string> Items { get; set; }
		}
	}
}