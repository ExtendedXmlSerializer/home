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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Encryption;
using ExtendedXmlSerializer.ExtensionModel.References;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using ExtendedXmlSerializer.Tests.TestObject;
using JetBrains.Annotations;
using Xunit;

namespace ExtendedXmlSerializer.Tests.Configuration
{
	[SuppressMessage("ReSharper", "TestFileNameWarning")]
	public class ExtendedConfigurationTests
	{
		const string Testclass = "UpdatedTestClassName", MemberName = "UpdatedMemberName";

		static IConfiguration Configure(Action<IConfiguration> configure)
		{
			var result = new ExtendedConfiguration();
			configure(result);
			return result;
		}

		[Fact]
		public void ConfigureType()
		{
			var config = Configure(cfg => cfg.ConfigureType<TestClassPrimitiveTypes>());

			var configType = config.GetTypeConfiguration(typeof(TestClassPrimitiveTypes));
			Assert.NotNull(configType);
			Assert.Same(configType, config.GetTypeConfiguration(typeof(TestClassPrimitiveTypes)));

			Assert.NotNull(config.GetTypeConfiguration(typeof(TestClassPrimitiveTypesNullable)));
		}

		[Fact]
		public void ConfigureNameForType()
		{
			var configuration = new ExtendedConfiguration();
			var sut = configuration.Type<SimpleTestSubject>().Name(Testclass);

			Assert.Equal(sut.Name(), Testclass);
			Assert.Null(configuration.Type<TestClassPrimitiveTypesNullable>().Name());

			var support = new SerializationSupport(configuration);
			var expected = new SimpleTestSubject {BasicProperty = "Hello World!"};
			var actual = support.Assert(expected,
			                            @"<?xml version=""1.0"" encoding=""utf-8""?><UpdatedTestClassName xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.Configuration;assembly=ExtendedXmlSerializer.Tests""><BasicProperty>Hello World!</BasicProperty></UpdatedTestClassName>");
			Assert.Equal(expected.BasicProperty, actual.BasicProperty);
		}

		[Fact]
		public void ConfigureEntityType()
		{
			var configuration = new ExtendedConfiguration();

			Assert.Null(configuration.Find<ReferencesExtension>());

			var configType = configuration.EnableReferences().GetTypeConfiguration(typeof(TestClassPrimitiveTypes));
			var extension = configuration.Find<ReferencesExtension>();
			Assert.NotNull(extension);
			Assert.Null(extension.Get(configType.Get()));
		}

		[Fact]
		public void ConfigureMigrationForType()
		{
			var configuration = Configure(cfg => cfg.ConfigureType<TestClassPrimitiveTypes>().AddMigration(xml => { }));
			var type = configuration.GetTypeConfiguration(typeof(TestClassPrimitiveTypes));
			Assert.Equal(configuration.With<MigrationsExtension>().Get(type.Get()).Count(), 1);
		}

		[Fact]
		public void ConfigureCustomSerializerForType()
		{
			var config = Configure(cfg =>
			                       {
				                       var t = cfg.ConfigureType<TestClassPrimitiveTypes>();
				                       Assert.Null(cfg.With<CustomXmlExtension>().Get(t.Get()));
				                       t.CustomSerializer((writer, types) => { }, element => null);
			                       });
			var type = config.GetTypeConfiguration(typeof(TestClassPrimitiveTypes));
			Assert.NotNull(config.With<CustomXmlExtension>().Get(type.Get()));
		}

		[Fact]
		public void ConfigureProperty()
		{
			var config = Configure(cfg => cfg.ConfigureType<TestClassPrimitiveTypes>().Member(p => p.PropChar));
			var configType = config.GetTypeConfiguration(typeof(TestClassPrimitiveTypes));
			Assert.NotNull(configType.Member("PropChar"));
			Assert.Null(configType.Member("TheNewPropertyThatDoesNotExist"));
		}

		[Fact]
		public void ConfigurePropertyAsId()
		{
			var configuration =
				Configure(cfg => cfg.ConfigureType<TestClassPrimitiveTypes>().EnableReferences(p => p.PropChar));
			var configType = configuration.GetTypeConfiguration(typeof(TestClassPrimitiveTypes));
			var property = configType.Member("PropChar");
			Assert.NotNull(property);

			var extension = configuration.Find<ReferencesExtension>();
			Assert.NotNull(extension);
			Assert.Same(property.Get(), extension.Get(configType.Get()));
		}

		[Fact]
		public void ConfigureNameForProperty()
		{
			var configuration =
				Configure(cfg => cfg.ConfigureType<SimpleTestSubject>().Member(p => p.BasicProperty).Name(MemberName));

			var member =
				configuration.GetTypeConfiguration(typeof(SimpleTestSubject)).Member(nameof(SimpleTestSubject.BasicProperty));
			Assert.Equal(member.Name(), MemberName);

			var support = new SerializationSupport(configuration);
			var instance = new SimpleTestSubject {BasicProperty = "Hello World!  Testing Member."};
			support.Assert(instance,
			               @"<?xml version=""1.0"" encoding=""utf-8""?><ExtendedConfigurationTests-SimpleTestSubject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.Configuration;assembly=ExtendedXmlSerializer.Tests""><UpdatedMemberName>Hello World!  Testing Member.</UpdatedMemberName></ExtendedConfigurationTests-SimpleTestSubject>");
		}

		[Fact]
		public void ConfigureOrderForProperty()
		{
			var order = 0;
			var configuration = Configure(cfg =>
				                              cfg.ConfigureType<SimpleOrderedTestSubject>().Member(p => p.Property2).Order(order));
			var member =
				configuration.GetTypeConfiguration(typeof(SimpleOrderedTestSubject))
				             .Member(nameof(SimpleOrderedTestSubject.Property2));
			Assert.Equal(member.Order(), order);

			var instance = new SimpleOrderedTestSubject {Property2 = "World!", Property1 = "Hello"};

			new SerializationSupport().Assert(instance,
			                                  @"<?xml version=""1.0"" encoding=""utf-8""?><ExtendedConfigurationTests-SimpleOrderedTestSubject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.Configuration;assembly=ExtendedXmlSerializer.Tests""><Property1>Hello</Property1><Property2>World!</Property2></ExtendedConfigurationTests-SimpleOrderedTestSubject>");

			new SerializationSupport(configuration).Assert(instance,
			                                               @"<?xml version=""1.0"" encoding=""utf-8""?><ExtendedConfigurationTests-SimpleOrderedTestSubject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.Configuration;assembly=ExtendedXmlSerializer.Tests""><Property2>World!</Property2><Property1>Hello</Property1></ExtendedConfigurationTests-SimpleOrderedTestSubject>");
		}

		[Fact]
		public void ConfigurePropertyAsAttribute()
		{
			var configuration =
				Configure(cfg => { cfg.ConfigureType<SimpleTestSubject>().Member(p => p.BasicProperty).Attribute(); });
			var member = configuration.GetTypeConfiguration(typeof(SimpleTestSubject))
			                          .Member(nameof(SimpleTestSubject.BasicProperty))
			                          .Get();
			Assert.True(configuration.With<MemberFormatExtension>().Registered.Contains(member));

			var instance = new SimpleTestSubject {BasicProperty = "Hello World as Attribute!"};
			new SerializationSupport(configuration).Assert(instance,
			                                               @"<?xml version=""1.0"" encoding=""utf-8""?><ExtendedConfigurationTests-SimpleTestSubject BasicProperty=""Hello World as Attribute!"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.Configuration;assembly=ExtendedXmlSerializer.Tests"" />");
		}

		[Fact]
		public void ConfigureEncrypt()
		{
			var before = new ExtendedConfiguration();
			Assert.Null(before.Find<EncryptionExtension>());
			var configuration = before
				.UseEncryptionAlgorithm()
			    .ConfigureType<TestClassWithEncryptedData>()
			    .Member(p => p.Password, x => x.Encrypt())
			    .Member(p => p.Salary).Encrypt().Owner.Configuration;

			var extension = configuration.Find<EncryptionExtension>();
			Assert.NotNull(extension);
			var type = configuration.GetTypeConfiguration(typeof(TestClassWithEncryptedData));
			Assert.NotNull(type);

			var property = type.Member(nameof(TestClassWithEncryptedData.Salary)).Get();
			Assert.NotNull(property);
			Assert.Contains(property, extension.Registered);

			const int salary = 6776;
			var instance = new TestClassWithEncryptedData { Salary = salary };
			var actual = new SerializationSupport(configuration).Assert(instance, @"<?xml version=""1.0"" encoding=""utf-8""?><TestClassWithEncryptedData Salary=""Njc3Ng=="" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests"" />");
			Assert.Equal(salary, actual.Salary);
		}

		class SimpleTestSubject
		{
			public string BasicProperty { get; set; }
		}

		class SimpleOrderedTestSubject
		{
			public string Property1 { [UsedImplicitly] get; set; }
			public string Property2 { get; set; }
		}
	}
}