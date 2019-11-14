using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.References;
using ExtendedXmlSerializer.Tests.Support;
using ExtendedXmlSerializer.Tests.TestObject;
using FluentAssertions;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ExtensionModel.References
{
	public class ReferencesExtensionTests
	{
		readonly static Guid Guid = new Guid("{6DBB618F-DBBD-4909-9644-A1D955F06249}");

		[Fact]
		public void SimpleIdentity()
		{
			var support = new SerializationSupport(new ConfigurationContainer().EnableReferences()
			                                                                   .Create());
			var instance = new Subject
				{Id = new Guid("{0E2DECA4-CC38-46BA-9C47-94B8070D7353}"), PropertyName = "Hello World!"};
			instance.Self = instance;
			var actual = support.Assert(instance,
			                            @"<?xml version=""1.0"" encoding=""utf-8""?><ReferencesExtensionTests-Subject xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:identity=""1"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.References;assembly=ExtendedXmlSerializer.Tests""><Id>0e2deca4-cc38-46ba-9c47-94b8070d7353</Id><Self exs:reference=""1"" /><PropertyName>Hello World!</PropertyName></ReferencesExtensionTests-Subject>");
			Assert.NotNull(actual.Self);
			Assert.Same(actual, actual.Self);
		}

		[Fact]
		public void EnabledWithoutConfiguration()
		{
			var support = new SerializationSupport(new ConfigurationContainer().EnableReferences()
			                                                                   .Create());
			var expected = new Subject
			{
				Id           = Guid,
				PropertyName = "Primary Root",
				Self         = new Subject {Id = Guid, PropertyName = "Another subject"}
			};
			var actual = support.Assert(expected,
			                            @"<?xml version=""1.0"" encoding=""utf-8""?><ReferencesExtensionTests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.References;assembly=ExtendedXmlSerializer.Tests""><Id>6dbb618f-dbbd-4909-9644-a1d955f06249</Id><Self><Id>6dbb618f-dbbd-4909-9644-a1d955f06249</Id><PropertyName>Another subject</PropertyName></Self><PropertyName>Primary Root</PropertyName></ReferencesExtensionTests-Subject>");
			Assert.NotNull(actual.Self);
			Assert.NotSame(actual, actual.Self);
			Assert.StrictEqual(expected.Id, actual.Id);
			Assert.StrictEqual(expected.Self.Id, actual.Self.Id);
			Assert.StrictEqual(expected.Id, expected.Self.Id);
		}

		[Fact]
		public void SimpleEntity()
		{
			var configuration = new ConfigurationContainer();
			configuration.Type<Subject>()
			             .Member(x => x.Id)
			             .Identity();
			var support = new SerializationSupport(configuration);
			var expected = new Subject
			{
				Id           = Guid,
				PropertyName = "Primary Root"
			};
			expected.Self = expected;
			var actual = support.Assert(expected,
			                            @"<?xml version=""1.0"" encoding=""utf-8""?><ReferencesExtensionTests-Subject Id=""6dbb618f-dbbd-4909-9644-a1d955f06249"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.References;assembly=ExtendedXmlSerializer.Tests""><Self xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:entity=""6dbb618f-dbbd-4909-9644-a1d955f06249"" /><PropertyName>Primary Root</PropertyName></ReferencesExtensionTests-Subject>");
			Assert.NotNull(actual.Self);
			Assert.Same(actual, actual.Self);
		}

		[Fact]
		public void ComplexInstance()
		{
			var configuration = new ConfigurationContainer();
			configuration.Type<TestClassReference>()
			             .EnableReferences(x => x.Id);
			var support = new SerializationSupport(configuration);

			var instance = new TestClassReference
			{
				Id      = 1,
				ObjectA = new TestClassReference {Id = 2}
			};
			instance.CyclicReference    = instance;
			instance.ReferenceToObjectA = instance.ObjectA;
			instance.Lists = new List<IReference>
			{
				new TestClassReference {Id = 3},
				new TestClassReference {Id = 4}
			};

			var actual = support.Assert(instance,
			                            @"<?xml version=""1.0"" encoding=""utf-8""?><TestClassReference Id=""1"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests""><CyclicReference xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:type=""TestClassReference"" exs:entity=""1"" /><ObjectA xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:type=""TestClassReference"" Id=""2"" /><ReferenceToObjectA xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:type=""TestClassReference"" exs:entity=""2"" /><Lists><Capacity>4</Capacity><TestClassReference Id=""3"" /><TestClassReference Id=""4"" /></Lists></TestClassReference>");
			Assert.NotNull(actual.ObjectA);
			Assert.Same(instance, instance.CyclicReference);
			Assert.Same(instance.ObjectA, instance.ReferenceToObjectA);
			Assert.Equal(3, instance.Lists.First()
			                        .Id);
			Assert.Equal(4, instance.Lists.Last()
			                        .Id);
		}

		[Fact]
		public void PropertyInterfaceOfList()
		{
			var expected = new ClassWithPropertyInterfaceOfList
			{
				List = new List<string> {"Item1"},
				Set  = new List<string> {"Item1"}
			};

			var actual = new SerializationSupport(new ConfigurationContainer()).Cycle(expected);
			actual.List.Only()
			      .Should()
			      .NotBeNull()
			      .And
			      .Be(actual.Set.Only());
		}

		[Fact]
		public void PropertyInterfaceOfListEnabledReferences()
		{
			var expected = new ClassWithPropertyInterfaceOfList
			{
				List = new List<string> {"Item1"},
				Set  = new List<string> {"Item1"}
			};

			var container = new ConfigurationContainer();
			container.EnableReferences();
			var actual = new SerializationSupport(container).Cycle(expected);
			actual.List.Only()
			      .Should()
			      .NotBeNull()
			      .And
			      .Be(actual.Set.Only());
		}

		[Fact]
		public void PropertyInterfaceOfListReferencesCleared()
		{
			var expected = new ClassWithPropertyInterfaceOfList
			{
				List = new List<string> {"Item1"},
				Set  = new List<string> {"Item1"}
			};

			var container = new ConfigurationContainer();
			container.EnableReferences();
			container.IgnoredReferenceTypes()
			         .Clear();
			var actual = new SerializationSupport(container).Cycle(expected);
			actual.List.Only()
			      .Should()
			      .NotBeNull()
			      .And.BeSameAs(actual.Set.Only());
		}

		[Fact]
		public void GeneralInheritance()
		{
			var first     = new ChildClass {Name = "Key"};
			var instance  = new Container {First = first, Second = first};
			var container = new ConfigurationContainer();
			container.EnableReferences();
			var support = new SerializationSupport(container);
			var actual  = support.Cycle(instance);
			actual.First.Should()
			      .NotBeNull()
			      .And.BeOfType<ChildClass>()
			      .And.BeSameAs(actual.Second);
		}

		[Fact]
		public void SpecificParentInheritance()
		{
			var first     = new ChildClass {Name = "Key"};
			var instance  = new Container {First = first, Second = first};
			var container = new ConfigurationContainer();
			container.Type<ParentClass>()
			         .EnableReferences(x => x.Name);
			var support = new SerializationSupport(container);
			var actual  = support.Cycle(instance);
			actual.First.Should()
			      .NotBeNull()
			      .And.BeOfType<ChildClass>()
			      .And.BeSameAs(actual.Second);
		}

		[Fact]
		public void SpecificChildInheritance()
		{
			var first     = new ChildClass {Name = "Key"};
			var instance  = new Container {First = first, Second = first};
			var container = new ConfigurationContainer();
			container.Type<ChildClass>()
			         .EnableReferences(x => x.Name);
			var support = new SerializationSupport(container);
			var actual  = support.Cycle(instance);
			actual.First.Should()
			      .NotBeNull()
			      .And.BeOfType<ChildClass>()
			      .And.BeSameAs(actual.Second);
		}

		class Container
		{
			public ChildClass First { get; set; }
			public ChildClass Second { get; set; }
		}

		public class ParentClass
		{
			public string Name { get; set; }
		}

		public class ChildClass : ParentClass {}

		class ClassWithPropertyInterfaceOfList
		{
			[UsedImplicitly]
			public IList<string> List { get; set; }

			[UsedImplicitly]
			public IList<string> Set { get; set; }
		};

		[Fact]
		public void ComplexList()
		{
			var container = new ConfigurationContainer();
			container.Type<TestClassReference>()
			         .EnableReferences(x => x.Id);
			var support = new SerializationSupport(container);

			var instance = new TestClassReferenceWithList {Parent = new TestClassReference {Id = 1}};
			var other = new TestClassReference
				{Id = 2, ObjectA = instance.Parent, ReferenceToObjectA = instance.Parent};
			instance.All = new List<IReference>
			{
				new TestClassReference {Id = 3, ObjectA = instance.Parent, ReferenceToObjectA = instance.Parent},
				new TestClassReference {Id = 4, ObjectA = other, ReferenceToObjectA           = other},
				other,
				instance.Parent
			};
			var actual = support.Assert(instance,
			                            @"<?xml version=""1.0"" encoding=""utf-8""?><TestClassReferenceWithList xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests""><Parent xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:type=""TestClassReference"" Id=""1"" /><All><Capacity>4</Capacity><TestClassReference Id=""3""><ObjectA xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:type=""TestClassReference"" exs:entity=""1"" /><ReferenceToObjectA xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:type=""TestClassReference"" exs:entity=""1"" /></TestClassReference><TestClassReference Id=""4""><ObjectA xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:type=""TestClassReference"" Id=""2""><ObjectA exs:type=""TestClassReference"" exs:entity=""1"" /><ReferenceToObjectA exs:type=""TestClassReference"" exs:entity=""1"" /></ObjectA><ReferenceToObjectA xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:type=""TestClassReference"" exs:entity=""2"" /></TestClassReference><TestClassReference xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:entity=""2"" /><TestClassReference xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:entity=""1"" /></All></TestClassReferenceWithList>");
			Assert.NotNull(actual.Parent);
			var list = actual.All.Cast<TestClassReference>()
			                 .ToList();
			Assert.Same(actual.Parent, list[0]
				            .ObjectA);
			Assert.Same(actual.Parent, list[0]
				            .ReferenceToObjectA);
			Assert.Same(list[1]
				            .ObjectA, list[1]
				            .ReferenceToObjectA);
			Assert.Same(list[1]
			            .ObjectA.To<TestClassReference>()
			            .ObjectA,
			            list[1]
				            .ObjectA.To<TestClassReference>()
				            .ReferenceToObjectA);
			Assert.Same(actual.Parent, list[1]
			                           .ObjectA.To<TestClassReference>()
			                           .ObjectA);
			Assert.Same(list[2], list[1]
				            .ObjectA);
			Assert.Same(actual.Parent, list[3]);
		}

		[Fact]
		public void Dictionary()
		{
			var container = new ConfigurationContainer();
			container.Type<TestClassReference>()
			         .EnableReferences(x => x.Id);
			var support = new SerializationSupport(container);

			var instance =
				new TestClassReferenceWithDictionary
				{
					Parent = new TestClassReference
					{
						Id   = 1,
						Name = "Hello World, this is a Name!"
					}
				};
			var other = new TestClassReference
				{Id = 2, ObjectA = instance.Parent, ReferenceToObjectA = instance.Parent};

			instance.All = new Dictionary<int, IReference>
			{
				{
					3, new TestClassReference {Id = 3, ObjectA = instance.Parent, ReferenceToObjectA = instance.Parent}
				},
				{4, new TestClassReference {Id = 4, ObjectA = other, ReferenceToObjectA = other}},
				{2, other},
				{1, instance.Parent}
			};

			var actual = support.Assert(instance,
			                            @"<?xml version=""1.0"" encoding=""utf-8""?><TestClassReferenceWithDictionary xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests""><Parent xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:type=""TestClassReference"" Id=""1""><Name>Hello World, this is a Name!</Name></Parent><All><Item xmlns=""https://extendedxmlserializer.github.io/system""><Key>3</Key><Value xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:type=""TestClassReference"" Id=""3""><ObjectA exs:type=""TestClassReference"" exs:entity=""1"" /><ReferenceToObjectA exs:type=""TestClassReference"" exs:entity=""1"" /></Value></Item><Item xmlns=""https://extendedxmlserializer.github.io/system""><Key>4</Key><Value xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:type=""TestClassReference"" Id=""4""><ObjectA exs:type=""TestClassReference"" Id=""2""><ObjectA exs:type=""TestClassReference"" exs:entity=""1"" /><ReferenceToObjectA exs:type=""TestClassReference"" exs:entity=""1"" /></ObjectA><ReferenceToObjectA exs:type=""TestClassReference"" exs:entity=""2"" /></Value></Item><Item xmlns=""https://extendedxmlserializer.github.io/system""><Key>2</Key><Value xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:type=""TestClassReference"" exs:entity=""2"" /></Item><Item xmlns=""https://extendedxmlserializer.github.io/system""><Key>1</Key><Value xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:type=""TestClassReference"" exs:entity=""1"" /></Item></All></TestClassReferenceWithDictionary>");
			Assert.NotNull(actual.Parent);
			var list = actual.All;
			Assert.Same(actual.Parent, list[3]
			                           .To<TestClassReference>()
			                           .ObjectA);
			Assert.Same(actual.Parent, list[3]
			                           .To<TestClassReference>()
			                           .ReferenceToObjectA);
			Assert.Same(list[4]
			            .To<TestClassReference>()
			            .ObjectA, list[4]
			                      .To<TestClassReference>()
			                      .ReferenceToObjectA);
			Assert.Same(list[4]
			            .To<TestClassReference>()
			            .ObjectA.To<TestClassReference>()
			            .ObjectA, actual.Parent);
			Assert.Same(list[4]
			            .To<TestClassReference>()
			            .ObjectA, list[2]);
			Assert.Same(actual.Parent, list[1]);
		}

		[Fact]
		public void OptimizedDictionary()
		{
			var container = new ConfigurationContainer();
			container.UseOptimizedNamespaces()
			         .Type<TestClassReference>()
			         .EnableReferences(x => x.Id);
			var support = container.ForTesting();

			var instance = new TestClassReferenceWithDictionary {Parent = new TestClassReference {Id = 1}};
			var other = new TestClassReference
				{Id = 2, ObjectA = instance.Parent, ReferenceToObjectA = instance.Parent};

			instance.All = new Dictionary<int, IReference>
			{
				{
					3, new TestClassReference {Id = 3, ObjectA = instance.Parent, ReferenceToObjectA = instance.Parent}
				},
				{4, new TestClassReference {Id = 4, ObjectA = other, ReferenceToObjectA = other}},
				{2, other},
				{1, instance.Parent}
			};

			var actual = support.Assert(instance,
			                            @"<?xml version=""1.0"" encoding=""utf-8""?><TestClassReferenceWithDictionary xmlns:exs=""https://extendedxmlserializer.github.io/v2"" xmlns:sys=""https://extendedxmlserializer.github.io/system"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests""><Parent exs:type=""TestClassReference"" Id=""1"" /><All><sys:Item><Key>3</Key><Value exs:type=""TestClassReference"" Id=""3""><ObjectA exs:type=""TestClassReference"" exs:entity=""1"" /><ReferenceToObjectA exs:type=""TestClassReference"" exs:entity=""1"" /></Value></sys:Item><sys:Item><Key>4</Key><Value exs:type=""TestClassReference"" Id=""4""><ObjectA exs:type=""TestClassReference"" Id=""2""><ObjectA exs:type=""TestClassReference"" exs:entity=""1"" /><ReferenceToObjectA exs:type=""TestClassReference"" exs:entity=""1"" /></ObjectA><ReferenceToObjectA exs:type=""TestClassReference"" exs:entity=""2"" /></Value></sys:Item><sys:Item><Key>2</Key><Value exs:type=""TestClassReference"" exs:entity=""2"" /></sys:Item><sys:Item><Key>1</Key><Value exs:type=""TestClassReference"" exs:entity=""1"" /></sys:Item></All></TestClassReferenceWithDictionary>");
			Assert.NotNull(actual.Parent);
			var list = actual.All;
			Assert.Same(actual.Parent, list[3]
			                           .To<TestClassReference>()
			                           .ObjectA);
			Assert.Same(actual.Parent, list[3]
			                           .To<TestClassReference>()
			                           .ReferenceToObjectA);
			Assert.Same(list[4]
			            .To<TestClassReference>()
			            .ObjectA, list[4]
			                      .To<TestClassReference>()
			                      .ReferenceToObjectA);
			Assert.Same(list[4]
			            .To<TestClassReference>()
			            .ObjectA.To<TestClassReference>()
			            .ObjectA, actual.Parent);
			Assert.Same(list[4]
			            .To<TestClassReference>()
			            .ObjectA, list[2]);
			Assert.Same(actual.Parent, list[1]);
		}

		[Fact]
		public void List()
		{
			var container = new ConfigurationContainer();
			container.Type<TestClassReference>()
			         .EnableReferences(x => x.Id);
			var support = new SerializationSupport(container);

			var parent = new TestClassReference {Id = 1};
			var other  = new TestClassReference {Id = 2, ObjectA = parent, ReferenceToObjectA = parent};

			var instance = new List<IReference>
			{
				new TestClassReference {Id = 3, ObjectA = parent, ReferenceToObjectA = parent},
				new TestClassReference {Id = 4, ObjectA = other, ReferenceToObjectA  = other},
				other,
				parent
			};

			var actual = support.Assert(instance,
			                            @"<?xml version=""1.0"" encoding=""utf-8""?><List xmlns:ns1=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests"" xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:arguments=""ns1:IReference"" xmlns=""https://extendedxmlserializer.github.io/system""><Capacity>4</Capacity><ns1:TestClassReference Id=""3""><ObjectA exs:type=""ns1:TestClassReference"" Id=""1"" /><ReferenceToObjectA exs:type=""ns1:TestClassReference"" exs:entity=""1"" /></ns1:TestClassReference><ns1:TestClassReference Id=""4""><ObjectA exs:type=""ns1:TestClassReference"" Id=""2""><ObjectA exs:type=""ns1:TestClassReference"" exs:entity=""1"" /><ReferenceToObjectA exs:type=""ns1:TestClassReference"" exs:entity=""1"" /></ObjectA><ReferenceToObjectA exs:type=""ns1:TestClassReference"" exs:entity=""2"" /></ns1:TestClassReference><ns1:TestClassReference exs:entity=""2"" /><ns1:TestClassReference exs:entity=""1"" /></List>");
			Assert.Same(actual[0]
			            .To<TestClassReference>()
			            .ObjectA, actual[0]
			                      .To<TestClassReference>()
			                      .ReferenceToObjectA);
			Assert.Same(actual[1]
			            .To<TestClassReference>()
			            .ObjectA, actual[1]
			                      .To<TestClassReference>()
			                      .ReferenceToObjectA);
			Assert.Same(actual[2], actual[1]
			                       .To<TestClassReference>()
			                       .ObjectA);
			Assert.Same(actual[3], actual[0]
			                       .To<TestClassReference>()
			                       .ObjectA);
		}

		[Fact]
		public void VerifyThrow()
		{
			Action sut = () =>
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
			             };

			sut.Should()
			   .Throw<CircularReferencesDetectedException>()
			   .WithMessage(@"The provided instance of type 'ExtendedXmlSerializer.Tests.ExtensionModel.References.ReferencesExtensionTests+Subject' contains circular references within its graph. Serializing this instance would result in a recursive, endless loop. To properly serialize this instance, please create a serializer that has referential support enabled by extending it with the ReferencesExtension.\r\n\r\nHere is a list of found references:\r\n- ExtendedXmlSerializer.Tests.ExtensionModel.References.ReferencesExtensionTests+Subject");
		}

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