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

namespace ExtendedXmlSerialization.Test.TestObject
{
	public enum TestEnum
	{
		EnumValue1,
		EnumValue2
	}

	public sealed class TestClassPrimitiveTypes
	{
		public static TestClassPrimitiveTypes Create()
		{
			var result = new TestClassPrimitiveTypes
			             {
				             PropString = "TestString",
				             PropInt = -1,
				             PropuInt = 2234,
				             PropDecimal = 3.346m,
				             PropDecimalMinValue = decimal.MinValue,
				             PropDecimalMaxValue = decimal.MaxValue,
				             PropFloat = 7.4432f,
				             PropFloatNaN = float.NaN,
				             PropFloatPositiveInfinity = float.PositiveInfinity,
				             PropFloatNegativeInfinity = float.NegativeInfinity,
				             PropFloatMinValue = float.MinValue,
				             PropFloatMaxValue = float.MaxValue,
				             PropDouble = 3.4234,
				             PropDoubleNaN = double.NaN,
				             PropDoublePositiveInfinity = double.PositiveInfinity,
				             PropDoubleNegativeInfinity = double.NegativeInfinity,
				             PropDoubleMinValue = double.MinValue,
				             PropDoubleMaxValue = double.MaxValue,
				             PropEnum = TestEnum.EnumValue1,
				             PropLong = 234234142,
				             PropUlong = 2345352534,
				             PropShort = 23,
				             PropUshort = 2344,
				             PropDateTime = new DateTime(2014, 01, 23),
				             PropByte = 23,
				             PropSbyte = 33,
				             PropChar = 'g',
			             };

			return result;
		}

		public string PropString { get; set; }
		public int PropInt { get; set; }
		public uint PropuInt { get; set; }
		public decimal PropDecimal { get; set; }
		public decimal PropDecimalMinValue { get; set; }
		public decimal PropDecimalMaxValue { get; set; }

		public float PropFloat { get; set; }
		public float PropFloatNaN { get; set; }
		public float PropFloatPositiveInfinity { get; set; }
		public float PropFloatNegativeInfinity { get; set; }
		public float PropFloatMinValue { get; set; }
		public float PropFloatMaxValue { get; set; }

		public double PropDouble { get; set; }
		public double PropDoubleNaN { get; set; }
		public double PropDoublePositiveInfinity { get; set; }
		public double PropDoubleNegativeInfinity { get; set; }
		public double PropDoubleMinValue { get; set; }
		public double PropDoubleMaxValue { get; set; }

		public TestEnum PropEnum { get; set; }
		public long PropLong { get; set; }
		public ulong PropUlong { get; set; }
		public short PropShort { get; set; }
		public ushort PropUshort { get; set; }
		public DateTime PropDateTime { get; set; }
		public byte PropByte { get; set; }
		public sbyte PropSbyte { get; set; }
		public char PropChar { get; set; }
	}
}