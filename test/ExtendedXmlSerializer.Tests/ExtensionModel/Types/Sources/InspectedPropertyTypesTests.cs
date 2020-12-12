using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using ExtendedXmlSerializer.ExtensionModel.Types.Sources;
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
		    public Engine Engine { get; set; }
	    }

	    class Engine
	    {
	    }

	    class AlmostEmptyClass
	    {
		    public int Int { get; set; }
		    public uint UInt { get; set; }
		    public string String { get; set; }
		    public long Long { get; set; }
		    public ulong ULong { get; set; }
		    public short Short { get; set; }
		    public ushort UShort { get; set; }
		    public double Double { get; set; }
		    public bool Bool { get; set; }
		    public decimal Decimal { get; set; }
		    public byte Byte { get; set; }
		    public sbyte SByte { get; set; }
		    public DateTime DateTime { get; set; }

			[XmlIgnore]
		    public Animal Animal { get; set; }
	    }

	    class Human
	    {
		    public Human Mother { get; set; }
		    public Human Father { get; set; }
	    }

	    class Zoo
	    {
		    public List<Animal> Animals { get; set; }
	    }

	    class Animal
	    {
	    }
	}


}
