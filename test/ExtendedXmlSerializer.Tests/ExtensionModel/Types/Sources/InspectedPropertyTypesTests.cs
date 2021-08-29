using ExtendedXmlSerializer.ExtensionModel.Types.Sources;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ExtensionModel.Types.Sources
{
	public class InspectedPropertyTypesTests
	{
		[Fact]
		public void ShouldHandleSingleProperty()
		{
			var types = new InspectedPropertyTypes<Vehicle>();

			var result = types.Get();

			Assert.Equal(2, result.Length);
			Assert.Contains(result, t => t == typeof(Vehicle));
			Assert.Contains(result, t => t == typeof(Engine));
		}

		[Fact]
		public void ShouldIgnoreExcludedProperties()
		{
			var types = new InspectedPropertyTypes<AlmostEmptyClass>();

			var result = types.Get();

			Assert.Single(result);
			Assert.Contains(result, t => t == typeof(AlmostEmptyClass));
		}

		[Fact]
		public void SamePropertyAsParentShouldNotCauseStackOverflow()
		{
			var types = new InspectedPropertyTypes<Human>();

			var result = types.Get();

			Assert.Single(result);
			Assert.Contains(result, t => t == typeof(Human));
		}

		[Fact]
		public void ShouldHandleListGracefully()
		{
			var types = new InspectedPropertyTypes<Zoo>();

			var result = types.Get();

			Assert.Equal(2, result.Length);
			Assert.Contains(result, t => t == typeof(Zoo));
			Assert.Contains(result, t => t == typeof(Animal));
		}

		class Vehicle
		{
			[UsedImplicitly]
			public Engine Engine { get; set; }
		}

		class Engine {}

		class AlmostEmptyClass
		{
			[UsedImplicitly]
			public int Int { get; set; }

			[UsedImplicitly]
			public uint UInt { get; set; }

			[UsedImplicitly]
			public string String { get; set; }

			[UsedImplicitly]
			public long Long { get; set; }

			[UsedImplicitly]
			public ulong ULong { get; set; }

			[UsedImplicitly]
			public short Short { get; set; }

			[UsedImplicitly]
			public ushort UShort { get; set; }

			[UsedImplicitly]
			public double Double { get; set; }

			[UsedImplicitly]
			public bool Bool { get; set; }

			[UsedImplicitly]
			public decimal Decimal { get; set; }

			[UsedImplicitly]
			public byte Byte { get; set; }

			[UsedImplicitly]
			public sbyte SByte { get; set; }

			[UsedImplicitly]
			public DateTime DateTime { get; set; }

			[XmlIgnore][UsedImplicitly]
			public Animal Animal { get; set; }
		}

		class Human
		{
			[UsedImplicitly]
			public Human Mother { get; set; }
			[UsedImplicitly]
			public Human Father { get; set; }
		}

		class Zoo
		{
			[UsedImplicitly]
			public List<Animal> Animals { get; set; }
		}

		class Animal {}
	}
}