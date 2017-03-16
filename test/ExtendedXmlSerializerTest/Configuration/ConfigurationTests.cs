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
using ExtendedXmlSerialization.Configuration;
using ExtendedXmlSerialization.Test.Support;
using ExtendedXmlSerialization.Test.TestObject;
using Xunit;

namespace ExtendedXmlSerialization.Test.Configuration
{
	[SuppressMessage("ReSharper", "TestFileNameWarning")]
	public class ConfigurationTests
	{
		const string Testclass = "UpdatedTestClassName";

		static IExtendedXmlConfiguration Configure(Action<IExtendedXmlConfiguration> configure)
		{
			var result = new ExtendedXmlConfiguration();
			configure(result);
			return result;
		}

		[Fact]
		public void ConfigureType()
		{
			var config = Configure(cfg =>
			{
				cfg.ConfigureType<TestClassPrimitiveTypes>();
			});

			var configType = config.GetTypeConfiguration(typeof(TestClassPrimitiveTypes));
			Assert.NotNull(configType);
			Assert.Same(configType, config.GetTypeConfiguration(typeof(TestClassPrimitiveTypes)));

			Assert.NotNull(config.GetTypeConfiguration(typeof(TestClassPrimitiveTypesNullable)));
		}

		[Fact]
		public void ConfigureNameForType()
		{
			var sut = Configure(cfg =>
			{
				cfg.ConfigureType<TestClassPrimitiveTypes>().Name(Testclass);
			});
			Assert.Equal(ConfiguredNames.Default.Get(sut.Type<TestClassPrimitiveTypes>()), Testclass);
			Assert.Null(ConfiguredNames.Default.Get(sut.Type<TestClassPrimitiveTypesNullable>()));

			var temp = new SerializationSupport(sut.Create());
			temp.WriteLine(TestClassPrimitiveTypes.Create());
		}

		/*[Fact]
		public void ConfigureEntityType()
		{
			var config = GetConfiguration(cfg =>
			{
				cfg.Add(new ReferencesExtension());
				cfg.ConfigureType<TestClassPrimitiveTypes>().EnableReferences();
			});
			var configType = config.GetTypeConfiguration(typeof(TestClassPrimitiveTypes));
			Assert.True(configType.IsObjectReference);
			Assert.Null(configType.GetObjectId);
		}

		[Fact]
		public void ConfigureMigrationForType()
		{
			var config = GetConfiguration(cfg =>
			{
				cfg.ConfigureType<TestClassPrimitiveTypes>().AddMigration((xml) => { });
			});
			var configType = config.GetTypeConfiguration(typeof(TestClassPrimitiveTypes));
			Assert.Equal(configType.Version, 1);
		}

		[Fact]
		public void ConfigureCustomSerializerForType()
		{
			var config = GetConfiguration(cfg =>
			{
				cfg.ConfigureType<TestClassPrimitiveTypes>().CustomSerializer((writer, types) => { }, element => null);
			});
			var configType = config.GetTypeConfiguration(typeof(TestClassPrimitiveTypes));
			Assert.True(configType.IsCustomSerializer);
		}

		[Fact]
		public void ConfigureProperty()
		{
			var config = GetConfiguration(cfg =>
			{
				cfg.ConfigureType<TestClassPrimitiveTypes>().Member(p=>p.PropChar);
			});
			var configType = config.GetTypeConfiguration(typeof(TestClassPrimitiveTypes));
			Assert.NotNull(configType.Member("PropChar"));
			Assert.Null(configType.Member("TheNewPropertyThatDoesNotExist"));
		}

		[Fact]
		public void ConfigureNameForProperty()
		{
			var config = GetConfiguration(cfg =>
			{
				cfg.ConfigureType<TestClassPrimitiveTypes>().Member(p=>p.PropChar).Name("Char");
			});
			var configType = config.GetTypeConfiguration(typeof(TestClassPrimitiveTypes));
			var property = configType.Member("PropChar");
			Assert.Equal(property.ChangedName, "Char");
		}

		[Fact]
		public void ConfigureOrderForProperty()
		{
			var config = GetConfiguration(cfg =>
			{
				cfg.ConfigureType<TestClassPrimitiveTypes>().Member(p=>p.PropChar).Order(3);
			});
			var configType = config.GetTypeConfiguration(typeof(TestClassPrimitiveTypes));
			var property = configType.Member("PropChar");
			Assert.Equal(property.ChangedOrder, 3);
		}

		[Fact]
		public void ConfigurePropertyAsAttribute()
		{
			var config = GetConfiguration(cfg =>
			{
				cfg.ConfigureType<TestClassPrimitiveTypes>().Member(p=>p.PropChar).AsAttribute();
			});
			var configType = config.GetTypeConfiguration(typeof(TestClassPrimitiveTypes));
			var property = configType.Member("PropChar");
			Assert.True(property.IsAttribute);
		}

		[Fact]
		public void ConfigurePropertyAsId()
		{
			var config = GetConfiguration(cfg =>
			{
				cfg.ConfigureType<TestClassPrimitiveTypes>().Member(p=>p.PropChar).EnableReferences();
			});
			var configType = config.GetTypeConfiguration(typeof(TestClassPrimitiveTypes));
			var property = configType.Member("PropChar");
			Assert.True(property.IsObjectReference);
			Assert.True(configType.IsObjectReference);
			Assert.NotNull(configType.GetObjectId);
		}*/

		[Fact]
		public void ConfigureEncrypt()
		{
			/*var config = GetConfiguration(cfg =>
			{
				cfg.ConfigureType<TestClassWithEncryptedData>()
					.Property(p => p.Password).Encrypt()
					.Property(p => p.Salary).Encrypt();

				cfg.UseEncryptionAlgorithm(Encryption.Default);
			});
			//Assert.NotNull(config.EncryptionAlgorithm);
			var configType = config.GetTypeConfiguration(typeof(TestClassWithEncryptedData));
			Assert.NotNull(configType);

			var salaryProperty = configType.GetPropertyConfiguration("Salary");
			Assert.NotNull(salaryProperty);
			Assert.True(salaryProperty.IsEncrypt);*/
		}
	}
}