using System;

// ReSharper disable All

namespace ExtendedXmlSerializer.Tests.TestObject
{
	public class TestClassPrimitiveTypesNullable
	{
		public void Init()
		{
			PropString   = "TestString";
			PropInt      = -1;
			PropuInt     = 2234;
			PropDecimal  = 3.346m;
			PropFloat    = 7.4432f;
			PropDouble   = 3.4234;
			PropEnum     = TestEnum.EnumValue1;
			PropLong     = 234234142;
			PropUlong    = 2345352534;
			PropShort    = 23;
			PropUshort   = 2344;
			PropDateTime = new DateTime(2014, 01, 23);
			PropByte     = 23;
			PropSbyte    = 33;
			PropChar     = 'g';
		}

		public void InitNull()
		{
			PropString   = null;
			PropInt      = null;
			PropuInt     = null;
			PropDecimal  = null;
			PropFloat    = null;
			PropDouble   = null;
			PropEnum     = null;
			PropLong     = null;
			PropUlong    = null;
			PropShort    = null;
			PropUshort   = null;
			PropDateTime = null;
			PropByte     = null;
			PropSbyte    = null;
			PropChar     = null;
		}

		public string PropString { get; set; }
		public int? PropInt { get; set; }
		public uint? PropuInt { get; set; }
		public decimal? PropDecimal { get; set; }
		public float? PropFloat { get; set; }
		public double? PropDouble { get; set; }
		public TestEnum? PropEnum { get; set; }
		public long? PropLong { get; set; }
		public ulong? PropUlong { get; set; }
		public short? PropShort { get; set; }
		public ushort? PropUshort { get; set; }
		public DateTime? PropDateTime { get; set; }
		public byte? PropByte { get; set; }
		public sbyte? PropSbyte { get; set; }
		public char? PropChar { get; set; }
	}
}