using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue218Tests
	{
		[Fact]
		void Verify()
		{
			var instance = new MyListImpl<string>("Hello World!") {"One", "Two"};
			new ConfigurationContainer().EnableParameterizedContent()
			                            .Create()
			                            .ForTesting()
			                            .Cycle(instance)
			                            .ShouldBeEquivalentTo(instance);
		}

		[Fact]
		void VerifyParent()
		{
			var instance = new SerializedObject();
			new ConfigurationContainer().EnableParameterizedContent()
			                            .EnableReferences()
			                            .UseOptimizedNamespaces()
			                            .Create()
			                            .ForTesting()
			                            .Assert(instance,
			                                    @"<?xml version=""1.0"" encoding=""utf-8""?><Issue218Tests-SerializedObject xmlns:sys=""https://extendedxmlserializer.github.io/system"" xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:identity=""1"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><MyListImpl><Owner exs:type=""Issue218Tests-SerializedObject"" exs:reference=""1"" /><Capacity>4</Capacity><sys:string>Test</sys:string><sys:string>One</sys:string><sys:string>Two</sys:string></MyListImpl></Issue218Tests-SerializedObject>");
		}

		public class MyListImpl<T> : List<T>
		{
			public MyListImpl(object owner) => Owner = owner;

			public object Owner { get; }
		}

		public class SerializedObject
		{
			public SerializedObject() => MyListImpl = new MyListImpl<string>(this) {"Test", "One", "Two"};

			public MyListImpl<string> MyListImpl { get; }
		}
	}
}