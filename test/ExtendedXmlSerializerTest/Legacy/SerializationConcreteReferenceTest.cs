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
using ExtendedXmlSerialization.Test.Legacy.TestObjectConfigs;
using ExtendedXmlSerialization.Test.TestObject;
using Xunit;

namespace ExtendedXmlSerialization.Test.Legacy
{
	public class SerializationConcreteReferenceTest : BaseTest
	{
		public SerializationConcreteReferenceTest()
		{
			/*Serializer = new Legacy.ExtendedXmlSerializer(cfg =>
			                                       {
				                                       cfg.ConfigureType<TestClassConcreteReference>()
				                                          .Property(p => p.Id)
				                                          .ObjectReference();
				                                       cfg.ConfigureType<TestClassReference>().Property(p => p.Id).ObjectReference();
				                                       cfg.ConfigureType<IReference>().Property(p => p.Id).ObjectReference();
			                                       });*/
			Serializer.SerializationToolsFactory = new SimpleSerializationToolsFactory()
			                                       {
				                                       Configurations =
					                                       new List<IExtendedXmlSerializerConfig>
					                                       {
						                                       new TestClassConcreteReferenceConfig
							                                       (),
						                                       new TestClassReferenceConfig(),
						                                       new InterfaceReferenceConfig()
					                                       }
			                                       };
		}

		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void SerializationRefernece()
		{
			var obj = new TestClassConcreteReference();
			obj.Id = 1;
			obj.CyclicReference = obj;
			obj.ObjectA = new TestClassConcreteReference {Id = 2};
			obj.ReferenceToObjectA = obj.ObjectA;
			obj.Lists = new List<TestClassConcreteReference>
			            {
				            new TestClassConcreteReference {Id = 3},
				            new TestClassConcreteReference {Id = 4}
			            };

			CheckSerializationAndDeserialization("ExtendedXmlSerializerTest.Legacy.Resources.TestClassConcreteReference.xml", obj);
		}

		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void SerializationListWithReference()
		{
			var obj = new TestClassConcreteReferenceWithList();
			obj.Parent = new TestClassConcreteReference {Id = 1};
			var other = new TestClassConcreteReference {Id = 2, ObjectA = obj.Parent, ReferenceToObjectA = obj.Parent};

			obj.All = new List<TestClassConcreteReference>
			          {
				          new TestClassConcreteReference {Id = 3, ObjectA = obj.Parent, ReferenceToObjectA = obj.Parent},
				          new TestClassConcreteReference {Id = 4, ObjectA = other, ReferenceToObjectA = other},
				          other,
				          obj.Parent
			          };

			CheckSerializationAndDeserialization("ExtendedXmlSerializerTest.Legacy.Resources.TestClassConcreteReferenceWithList.xml",
			                                     obj);
		}

		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void SerializationDictionaryWithReference()
		{
			var obj = new TestClassConcreteReferenceWithDictionary();
			obj.Parent = new TestClassConcreteReference {Id = 1};
			var other = new TestClassConcreteReference {Id = 2, ObjectA = obj.Parent, ReferenceToObjectA = obj.Parent};

			obj.All = new Dictionary<int, TestClassConcreteReference>
			          {
				          {3, new TestClassConcreteReference {Id = 3, ObjectA = obj.Parent, ReferenceToObjectA = obj.Parent}},
				          {4, new TestClassConcreteReference {Id = 4, ObjectA = other, ReferenceToObjectA = other}},
				          {2, other},
				          {1, obj.Parent}
			          };

			CheckSerializationAndDeserialization(
				"ExtendedXmlSerializerTest.Legacy.Resources.TestClassConcreteReferenceWithDictionary.xml", obj);
		}

		[Fact, Trait(Traits.Category, Traits.Categories.Legacy)]
		public void SerializationListOfInterfaceReference()
		{
			var parent = new TestClassConcreteReference {Id = 1};
			var other = new TestClassConcreteReference {Id = 2, ObjectA = parent, ReferenceToObjectA = parent};

			var obj = new List<TestClassConcreteReference>();
			obj.Add(new TestClassConcreteReference {Id = 3, ObjectA = parent, ReferenceToObjectA = parent});
			obj.Add(new TestClassConcreteReference {Id = 4, ObjectA = other, ReferenceToObjectA = other});
			obj.Add(other);
			obj.Add(parent);

			CheckSerializationAndDeserialization("ExtendedXmlSerializerTest.Legacy.Resources.ListOfConcreteReference.xml", obj);
		}
	}
}