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
using ExtendedXmlSerialization.Legacy;
using ExtendedXmlSerialization.Test.TestObject;
using Xunit;
#pragma warning disable 618

namespace ExtendedXmlSerialization.Test.Legacy
{
	// ReSharper disable once TestClassNameSuffixWarning
	public class SerializationPropertyEncrypt : BaseTest
	{
		public SerializationPropertyEncrypt()
		{
			Serializer.SerializationToolsFactory = new SimpleSerializationToolsFactory()
			                                       {
				                                       Configurations =
					                                       new List<IExtendedXmlSerializerConfig>
					                                       {
						                                       new TestClassWithEncryptedDataConfig(),
						                                       new TestClassWithEncryptedDataConfig()
					                                       },
				                                       EncryptionAlgorithm = new Base64PropertyEncryption()
			                                       };
		}

		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void SerializationRefernece()
		{
			var obj = new TestClassWithEncryptedData();
			obj.Name = "Name";
			obj.Password = "Cxl2983Hd";
			obj.Salary = 100000;

			CheckSerializationAndDeserialization("ExtendedXmlSerializerTest.Legacy.Resources.TestClassWithEncryptedData.xml", obj);
		}
	}
}