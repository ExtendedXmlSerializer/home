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

using System.Collections.Generic;
using ExtendedXmlSerialization;
using ExtendedXmlSerializer.Tests.Legacy.TestObject;
using ExtendedXmlSerializer.Tests.Legacy.TestObjectConfigs;
using Xunit;

#pragma warning disable 618

namespace ExtendedXmlSerializer.Tests.Legacy
{
	// ReSharper disable once TestClassNameSuffixWarning
	public class SerializationInheritanceWithCustomSerializerTest : BaseTest
	{
		public SerializationInheritanceWithCustomSerializerTest()
		{
			Serializer.SerializationToolsFactory = new SimpleSerializationToolsFactory()
												   {
													   Configurations = new List<IExtendedXmlSerializerConfig>
																		{
																			new TestClassInheritanceWithCustomSerializerBaseConfig(),
																			new TestClassInheritanceWithCustomSerializerAConfig(),
																			new TestClassInheritanceWithCustomSerializerBConfig()
																		}
												   };
		}

		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void MigrationsForInheritance()
		{
			TestClassInheritanceWithCustomSerializerBase obj = new TestClassInheritanceWithCustomSerializerA
			{
				FirstProperty = "1",
				SecondProperty = "2"
			};

			CheckSerializationAndDeserializationByXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
<TestClassInheritanceWithCustomSerializerA type=""ExtendedXmlSerializer.Tests.Legacy.TestObject.TestClassInheritanceWithCustomSerializerA"" First=""1"" Second=""2"" />", obj);

			obj = new TestClassInheritanceWithCustomSerializerBase()
			{
				FirstProperty = "1"
			};

			CheckSerializationAndDeserializationByXml(
					@"<?xml version=""1.0"" encoding=""utf-8""?>
<TestClassInheritanceWithCustomSerializerBase type=""ExtendedXmlSerializer.Tests.Legacy.TestObject.TestClassInheritanceWithCustomSerializerBase"" First=""1"" />", obj);

			obj = new TestClassInheritanceWithCustomSerializerB()
			{
				FirstProperty = "1",
				ThirdProperty = "3"
			};

			CheckSerializationAndDeserializationByXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
<TestClassInheritanceWithCustomSerializerB type=""ExtendedXmlSerializer.Tests.Legacy.TestObject.TestClassInheritanceWithCustomSerializerB""  First=""1"" Third=""3"" />", obj);

		}
	}
}