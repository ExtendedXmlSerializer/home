using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Tests.Support;
using ExtendedXmlSerializer.Tests.TestObject;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ExtensionModel.References
{
	public class DeferredReferencesExtensionTests
	{
		[Fact]
		public void ComplexListConfigUseFluentApi()
		{
			var container = new ConfigurationContainer();
			container.EnableDeferredReferences()
			         .Type<TestClassReference>()
			         .EnableReferences(x => x.Id);

			DoTest(new SerializationSupport(container));
		}

		[Fact]
		public void ComplexList()
		{
			var container = new ConfigurationContainer();
			container.Type<TestClassReference>()
			         .EnableReferences(x => x.Id);
			container.EnableDeferredReferences();
			DoTest(new SerializationSupport(container));
		}

		void DoTest(SerializationSupport support)
		{
			var instance = new TestClassReferenceWithList {Parent = new TestClassReference {Id = 1}};
			var other = new TestClassReference
				{Id = 2, ObjectA = instance.Parent, ReferenceToObjectA = instance.Parent};
			instance.All = new List<IReference>
			{
				new TestClassReference
				{
					Id                 = 3,
					ObjectA            = instance.Parent,
					ReferenceToObjectA = instance.Parent
				},
				new TestClassReference {Id = 4, ObjectA = other, ReferenceToObjectA = other},
				other,
				instance.Parent
			};

			var actual = support.Assert(
			                            instance,
			                            @"<?xml version=""1.0"" encoding=""utf-8""?><TestClassReferenceWithList xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests""><Parent xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:type=""TestClassReference"" Id=""1"" /><All><Capacity>4</Capacity><TestClassReference Id=""3""><ObjectA xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:type=""TestClassReference"" exs:entity=""1"" /><ReferenceToObjectA xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:type=""TestClassReference"" exs:entity=""1"" /></TestClassReference><TestClassReference Id=""4""><ObjectA xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:type=""TestClassReference"" exs:entity=""2"" /><ReferenceToObjectA xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:type=""TestClassReference"" exs:entity=""2"" /></TestClassReference><TestClassReference Id=""2""><ObjectA xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:type=""TestClassReference"" exs:entity=""1"" /><ReferenceToObjectA xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:type=""TestClassReference"" exs:entity=""1"" /></TestClassReference><TestClassReference xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:entity=""1"" /></All></TestClassReferenceWithList>");
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
			var reference = list[1]
			                .ObjectA.To<TestClassReference>();
			Assert.Same(reference.ObjectA, reference.ReferenceToObjectA);
			Assert.Same(actual.Parent, reference.ObjectA);
			Assert.Same(list[2], list[1]
				            .ObjectA);
			Assert.Same(actual.Parent, list[3]);
		}
	}
}