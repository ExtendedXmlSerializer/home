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
using System.Collections.Generic;
using System.Linq;
using ExtendedXmlSerialization.Configuration;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.ExtensionModel;
using ExtendedXmlSerialization.Test.Support;
using ExtendedXmlSerialization.Test.TestObject;
using JetBrains.Annotations;
using Xunit;

namespace ExtendedXmlSerialization.Test.ExtensionModel
{
	public class ReferencesExtensionTests
	{
		readonly static Guid Guid = new Guid("{6DBB618F-DBBD-4909-9644-A1D955F06249}");

		[Fact]
		public void SimpleIdentity()
		{
			var support = new SerializationSupport(new ExtendedConfiguration().Extend(new ReferencesExtension()).Create());
			var instance = new Subject {Id = new Guid("{0E2DECA4-CC38-46BA-9C47-94B8070D7353}"), PropertyName = "Hello World!"};
			instance.Self = instance;
			var actual = support.Assert(instance,
			                            @"<?xml version=""1.0"" encoding=""utf-8""?><ReferencesExtensionTests-Subject xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:identity=""1"" xmlns=""clr-namespace:ExtendedXmlSerialization.Test.ExtensionModel;assembly=ExtendedXmlSerializerTest""><Id>0e2deca4-cc38-46ba-9c47-94b8070d7353</Id><Self exs:reference=""1"" /><PropertyName>Hello World!</PropertyName></ReferencesExtensionTests-Subject>");
			Assert.NotNull(actual.Self);
			Assert.Same(actual, actual.Self);
		}

		[Fact]
		public void EnabledWithoutConfiguration()
		{
			var support = new SerializationSupport(new ExtendedConfiguration().Extend(new ReferencesExtension()).Create());
			var expected = new Subject
			               {
				               Id = Guid,
				               PropertyName = "Primary Root",
				               Self = new Subject {Id = Guid, PropertyName = "Another subject"}
			               };
			var actual = support.Assert(expected,
			                            @"<?xml version=""1.0"" encoding=""utf-8""?><ReferencesExtensionTests-Subject xmlns=""clr-namespace:ExtendedXmlSerialization.Test.ExtensionModel;assembly=ExtendedXmlSerializerTest""><Id>6dbb618f-dbbd-4909-9644-a1d955f06249</Id><Self><Id>6dbb618f-dbbd-4909-9644-a1d955f06249</Id><PropertyName>Another subject</PropertyName></Self><PropertyName>Primary Root</PropertyName></ReferencesExtensionTests-Subject>");
			Assert.NotNull(actual.Self);
			Assert.NotSame(actual, actual.Self);
			Assert.StrictEqual(expected.Id, actual.Id);
			Assert.StrictEqual(expected.Self.Id, actual.Self.Id);
			Assert.StrictEqual(expected.Id, expected.Self.Id);
		}

		[Fact]
		public void SimpleEntity()
		{
			var configuration = new ExtendedConfiguration().Type<Subject>().Member(x => x.Id).Identity().Configuration;
			var support = new SerializationSupport(configuration);
			var expected = new Subject
			               {
				               Id = Guid,
				               PropertyName = "Primary Root"
			               };
			expected.Self = expected;
			var actual = support.Assert(expected,
			                            @"<?xml version=""1.0"" encoding=""utf-8""?><ReferencesExtensionTests-Subject Id=""6dbb618f-dbbd-4909-9644-a1d955f06249"" xmlns=""clr-namespace:ExtendedXmlSerialization.Test.ExtensionModel;assembly=ExtendedXmlSerializerTest""><Self xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:entity=""6dbb618f-dbbd-4909-9644-a1d955f06249"" /><PropertyName>Primary Root</PropertyName></ReferencesExtensionTests-Subject>");
			Assert.NotNull(actual.Self);
			Assert.Same(actual, actual.Self);
		}

		[Fact]
		public void ComplexInstance()
		{
			var support =
				new SerializationSupport(
					new ExtendedConfiguration().Type<TestClassReference>().EnableReferences(x => x.Id).Configuration);

			var instance = new TestClassReference
			               {
				               Id = 1,
				               ObjectA = new TestClassReference {Id = 2}
			               };
			instance.CyclicReference = instance;
			instance.ReferenceToObjectA = instance.ObjectA;
			instance.Lists = new List<IReference>
			                 {
				                 new TestClassReference {Id = 3},
				                 new TestClassReference {Id = 4}
			                 };

			var actual = support.Assert(instance,
			                            @"<?xml version=""1.0"" encoding=""utf-8""?><TestClassReference Id=""1"" xmlns=""clr-namespace:ExtendedXmlSerialization.Test.TestObject;assembly=ExtendedXmlSerializerTest""><CyclicReference xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:type=""TestClassReference"" exs:entity=""1"" /><ObjectA xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:type=""TestClassReference"" Id=""2"" /><ReferenceToObjectA xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:type=""TestClassReference"" exs:entity=""2"" /><Lists><Capacity>4</Capacity><TestClassReference Id=""3"" /><TestClassReference Id=""4"" /></Lists></TestClassReference>");
			Assert.NotNull(actual.ObjectA);
			Assert.Same(instance, instance.CyclicReference);
			Assert.Same(instance.ObjectA, instance.ReferenceToObjectA);
			Assert.Equal(3, instance.Lists.First().Id);
			Assert.Equal(4, instance.Lists.Last().Id);
		}

		[Fact]
		public void ComplexList()
		{
			var support =
				new SerializationSupport(
					new ExtendedConfiguration().Type<TestClassReference>().EnableReferences(x => x.Id).Configuration);

			var instance = new TestClassReferenceWithList {Parent = new TestClassReference {Id = 1}};
			var other = new TestClassReference {Id = 2, ObjectA = instance.Parent, ReferenceToObjectA = instance.Parent};
			instance.All = new List<IReference>
			               {
				               new TestClassReference {Id = 3, ObjectA = instance.Parent, ReferenceToObjectA = instance.Parent},
				               new TestClassReference {Id = 4, ObjectA = other, ReferenceToObjectA = other},
				               other,
				               instance.Parent
			               };
			var actual = support.Assert(instance,
			                            @"<?xml version=""1.0"" encoding=""utf-8""?><TestClassReferenceWithList xmlns=""clr-namespace:ExtendedXmlSerialization.Test.TestObject;assembly=ExtendedXmlSerializerTest""><Parent xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:type=""TestClassReference"" Id=""1"" /><All><Capacity>4</Capacity><TestClassReference Id=""3""><ObjectA xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:type=""TestClassReference"" exs:entity=""1"" /><ReferenceToObjectA xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:type=""TestClassReference"" exs:entity=""1"" /></TestClassReference><TestClassReference Id=""4""><ObjectA xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:type=""TestClassReference"" Id=""2""><ObjectA exs:type=""TestClassReference"" exs:entity=""1"" /><ReferenceToObjectA exs:type=""TestClassReference"" exs:entity=""1"" /></ObjectA><ReferenceToObjectA xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:type=""TestClassReference"" exs:entity=""2"" /></TestClassReference><TestClassReference xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:entity=""2"" /><TestClassReference xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:entity=""1"" /></All></TestClassReferenceWithList>");
			Assert.NotNull(actual.Parent);
			var list = actual.All.Cast<TestClassReference>().ToList();
			Assert.Same(actual.Parent, list[0].ObjectA);
			Assert.Same(actual.Parent, list[0].ReferenceToObjectA);
			Assert.Same(list[1].ObjectA, list[1].ReferenceToObjectA);
			Assert.Same(list[1].ObjectA.To<TestClassReference>().ObjectA,
			            list[1].ObjectA.To<TestClassReference>().ReferenceToObjectA);
			Assert.Same(actual.Parent, list[1].ObjectA.To<TestClassReference>().ObjectA);
			Assert.Same(list[2], list[1].ObjectA);
			Assert.Same(actual.Parent, list[3]);
		}

		[Fact]
		public void Dictionary()
		{
			var support =
				new SerializationSupport(
					new ExtendedConfiguration().Type<TestClassReference>().EnableReferences(x => x.Id).Configuration);

			var instance = new TestClassReferenceWithDictionary {Parent = new TestClassReference {Id = 1}};
			var other = new TestClassReference {Id = 2, ObjectA = instance.Parent, ReferenceToObjectA = instance.Parent};

			instance.All = new Dictionary<int, IReference>
			               {
				               {3, new TestClassReference {Id = 3, ObjectA = instance.Parent, ReferenceToObjectA = instance.Parent}},
				               {4, new TestClassReference {Id = 4, ObjectA = other, ReferenceToObjectA = other}},
				               {2, other},
				               {1, instance.Parent}
			               };
			var actual = support.Assert(instance,
			                            @"<?xml version=""1.0"" encoding=""utf-8""?><TestClassReferenceWithDictionary xmlns=""clr-namespace:ExtendedXmlSerialization.Test.TestObject;assembly=ExtendedXmlSerializerTest""><Parent xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:type=""TestClassReference"" Id=""1"" /><All><Item xmlns=""https://github.com/wojtpl2/ExtendedXmlSerializer/system""><Key>3</Key><Value xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:type=""TestClassReference"" Id=""3""><ObjectA exs:type=""TestClassReference"" exs:entity=""1"" /><ReferenceToObjectA exs:type=""TestClassReference"" exs:entity=""1"" /></Value></Item><Item xmlns=""https://github.com/wojtpl2/ExtendedXmlSerializer/system""><Key>4</Key><Value xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:type=""TestClassReference"" Id=""4""><ObjectA exs:type=""TestClassReference"" Id=""2""><ObjectA exs:type=""TestClassReference"" exs:entity=""1"" /><ReferenceToObjectA exs:type=""TestClassReference"" exs:entity=""1"" /></ObjectA><ReferenceToObjectA exs:type=""TestClassReference"" exs:entity=""2"" /></Value></Item><Item xmlns=""https://github.com/wojtpl2/ExtendedXmlSerializer/system""><Key>2</Key><Value xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:type=""TestClassReference"" exs:entity=""2"" /></Item><Item xmlns=""https://github.com/wojtpl2/ExtendedXmlSerializer/system""><Key>1</Key><Value xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:type=""TestClassReference"" exs:entity=""1"" /></Item></All></TestClassReferenceWithDictionary>");
			Assert.NotNull(actual.Parent);
			var list = actual.All;
			Assert.Same(actual.Parent, list[3].To<TestClassReference>().ObjectA);
			Assert.Same(actual.Parent, list[3].To<TestClassReference>().ReferenceToObjectA);
			Assert.Same(list[4].To<TestClassReference>().ObjectA, list[4].To<TestClassReference>().ReferenceToObjectA);
			Assert.Same(list[4].To<TestClassReference>().ObjectA.To<TestClassReference>().ObjectA, actual.Parent);
			Assert.Same(list[4].To<TestClassReference>().ObjectA, list[2]);
			Assert.Same(actual.Parent, list[1]);
		}

		[Fact]
		public void List()
		{
			var support =
				new SerializationSupport(
					new ExtendedConfiguration().Type<TestClassReference>().EnableReferences(x => x.Id).Configuration);

			var parent = new TestClassReference {Id = 1};
			var other = new TestClassReference {Id = 2, ObjectA = parent, ReferenceToObjectA = parent};

			var instance = new List<IReference>
			               {
				               new TestClassReference {Id = 3, ObjectA = parent, ReferenceToObjectA = parent},
				               new TestClassReference {Id = 4, ObjectA = other, ReferenceToObjectA = other},
				               other,
				               parent
			               };

			var actual = support.Assert(instance,
			                            @"<?xml version=""1.0"" encoding=""utf-8""?><List xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" xmlns:ns1=""clr-namespace:ExtendedXmlSerialization.Test.TestObject;assembly=ExtendedXmlSerializerTest"" exs:arguments=""ns1:IReference"" xmlns=""https://github.com/wojtpl2/ExtendedXmlSerializer/system""><Capacity>4</Capacity><ns1:TestClassReference Id=""3""><ObjectA exs:type=""ns1:TestClassReference"" Id=""1"" /><ReferenceToObjectA exs:type=""ns1:TestClassReference"" exs:entity=""1"" /></ns1:TestClassReference><ns1:TestClassReference Id=""4""><ObjectA exs:type=""ns1:TestClassReference"" Id=""2""><ObjectA exs:type=""ns1:TestClassReference"" exs:entity=""1"" /><ReferenceToObjectA exs:type=""ns1:TestClassReference"" exs:entity=""1"" /></ObjectA><ReferenceToObjectA exs:type=""ns1:TestClassReference"" exs:entity=""2"" /></ns1:TestClassReference><ns1:TestClassReference exs:entity=""2"" /><ns1:TestClassReference exs:entity=""1"" /></List>");
			Assert.Same(actual[0].To<TestClassReference>().ObjectA, actual[0].To<TestClassReference>().ReferenceToObjectA);
			Assert.Same(actual[1].To<TestClassReference>().ObjectA, actual[1].To<TestClassReference>().ReferenceToObjectA);
			Assert.Same(actual[2], actual[1].To<TestClassReference>().ObjectA);
			Assert.Same(actual[3], actual[0].To<TestClassReference>().ObjectA);
		}

		[Fact]
		public void VerifyThrow()
			=> Assert.Throws<CircularReferencesDetectedException>(() =>
			                                                      {
				                                                      var support = new SerializationSupport();
				                                                      var instance = new Subject
				                                                                     {
					                                                                     Id =
						                                                                     new Guid(
							                                                                     "{0E2DECA4-CC38-46BA-9C47-94B8070D7353}"),
					                                                                     PropertyName = "Hello World!"
				                                                                     };
				                                                      instance.Self = instance;
				                                                      support.Serialize(instance);
			                                                      });

		class Subject
		{
			[UsedImplicitly]
			public Guid Id { get; set; }

			[UsedImplicitly]
			public Subject Self { get; set; }

			[UsedImplicitly]
			public string PropertyName { get; set; }
		}
	}
}