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
using ExtendedXmlSerializer.ExtensionModel.Xml;
using FluentAssertions;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.ComponentModel;
using Xunit;

// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace ExtendedXmlSerializer.Tests.ExtensionModel.Xml
{
	public class OptimizedNamespaceExtensionTests
	{
		[Fact]
		public void Optimized()
		{
			const string message = "Hello World!  This is a value set in a property with a variable type.",
				expected = @"<?xml version=""1.0"" encoding=""utf-8""?><OptimizedNamespaceExtensionTests-ClassWithDifferingPropertyType xmlns:exs=""https://extendedxmlserializer.github.io/v2"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Xml;assembly=ExtendedXmlSerializer.Tests""><Interface exs:type=""OptimizedNamespaceExtensionTests-Implementation""><PropertyName>Hello World!  This is a value set in a property with a variable type.</PropertyName></Interface></OptimizedNamespaceExtensionTests-ClassWithDifferingPropertyType>";
			var instance = new ClassWithDifferingPropertyType { Interface = new Implementation { PropertyName = message } };
			var serializer = new ConfigurationContainer().UseOptimizedNamespaces().Create();
			var data = serializer.Serialize(instance);
			Assert.Equal(expected, data);
		}

		[Fact]
		public void OptimizedList()
		{
			const string expected =
#if CORE
			@"<?xml version=""1.0"" encoding=""utf-8""?><OptimizedNamespaceExtensionTests-ClassWithDifferingPropertyType xmlns:exs=""https://extendedxmlserializer.github.io/v2"" xmlns:ns1=""clr-namespace:System.Collections.Generic;assembly=System.Collections"" xmlns:sys=""https://extendedxmlserializer.github.io/system"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Xml;assembly=ExtendedXmlSerializer.Tests""><Interface exs:type=""OptimizedNamespaceExtensionTests-GeneralImplementation""><Instance exs:type=""ns1:HashSet[sys:string]""><sys:string>Hello</sys:string><sys:string>World</sys:string><sys:string>Hope</sys:string><sys:string>This</sys:string><sys:string>Works!</sys:string></Instance></Interface></OptimizedNamespaceExtensionTests-ClassWithDifferingPropertyType>";
#else
			@"<?xml version=""1.0"" encoding=""utf-8""?><OptimizedNamespaceExtensionTests-ClassWithDifferingPropertyType xmlns:exs=""https://extendedxmlserializer.github.io/v2"" xmlns:ns1=""clr-namespace:System.Collections.Generic;assembly=System.Core"" xmlns:sys=""https://extendedxmlserializer.github.io/system"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Xml;assembly=ExtendedXmlSerializer.Tests""><Interface exs:type=""OptimizedNamespaceExtensionTests-GeneralImplementation""><Instance exs:type=""ns1:HashSet[sys:string]""><sys:string>Hello</sys:string><sys:string>World</sys:string><sys:string>Hope</sys:string><sys:string>This</sys:string><sys:string>Works!</sys:string></Instance></Interface></OptimizedNamespaceExtensionTests-ClassWithDifferingPropertyType>";
#endif


			var instance = new ClassWithDifferingPropertyType { Interface = new GeneralImplementation { Instance = new HashSet<string> {"Hello", "World", "Hope", "This", "Works!"} } };
			var serializer = new ConfigurationContainer().UseOptimizedNamespaces().Create();
			serializer.Serialize(instance).Should().Be(expected);
			serializer.Serialize(instance).Should().Be(expected);
		}

		[Fact]
		public void OptimizedDictionary()
		{
			const string expected =
#if CORE
			@"<?xml version=""1.0"" encoding=""utf-8""?><OptimizedNamespaceExtensionTests-ClassWithDifferingPropertyType xmlns:exs=""https://extendedxmlserializer.github.io/v2"" xmlns:sys=""https://extendedxmlserializer.github.io/system"" xmlns:ns1=""clr-namespace:JetBrains.Annotations;assembly=JetBrains.Annotations"" xmlns:ns2=""clr-namespace:System.ComponentModel;assembly=System.ComponentModel"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Xml;assembly=ExtendedXmlSerializer.Tests""><Interface exs:type=""OptimizedNamespaceExtensionTests-GeneralImplementation""><Instance exs:type=""sys:Dictionary[sys:Object,sys:Object]""><sys:Item><Key exs:type=""ns2:CancelEventArgs""><Cancel>false</Cancel></Key><Value exs:type=""ns1:UsedImplicitlyAttribute"" /></sys:Item></Instance></Interface></OptimizedNamespaceExtensionTests-ClassWithDifferingPropertyType>";
#else
			@"<?xml version=""1.0"" encoding=""utf-8""?><OptimizedNamespaceExtensionTests-ClassWithDifferingPropertyType xmlns:exs=""https://extendedxmlserializer.github.io/v2"" xmlns:sys=""https://extendedxmlserializer.github.io/system"" xmlns:ns1=""clr-namespace:JetBrains.Annotations;assembly=JetBrains.Annotations"" xmlns:ns2=""clr-namespace:System.ComponentModel;assembly=System"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Xml;assembly=ExtendedXmlSerializer.Tests""><Interface exs:type=""OptimizedNamespaceExtensionTests-GeneralImplementation""><Instance exs:type=""sys:Dictionary[sys:Object,sys:Object]""><sys:Item><Key exs:type=""ns2:CancelEventArgs""><Cancel>false</Cancel></Key><Value exs:type=""ns1:UsedImplicitlyAttribute"" /></sys:Item></Instance></Interface></OptimizedNamespaceExtensionTests-ClassWithDifferingPropertyType>";
#endif


			var instance = new ClassWithDifferingPropertyType
			               {
				               Interface = new GeneralImplementation
				                           {
					                           Instance = new Dictionary<object, object>
					                                      {
						                                      {new CancelEventArgs(), new UsedImplicitlyAttribute()}
					                                      }
				                           }
			               };
			var serializer = new ConfigurationContainer().UseOptimizedNamespaces().Create();
			var data = serializer.Serialize(instance);
			data.Should()
			    .Be(expected);
		}

		class ClassWithDifferingPropertyType
		{
			public IInterface Interface { get; set; }
		}

		public interface IInterface {}

		class Implementation : IInterface
		{
			public string PropertyName { get; set; }
		}

		class GeneralImplementation : IInterface
		{
			public object Instance { get; set; }
		}
	}
}