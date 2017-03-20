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
using System.Xml;
using ExtendedXmlSerialization.Cache;

namespace ExtendedXmlSerialization
{
	static class PrimitiveValueTools
	{
		public static string SetPrimitiveValue(object value, TypeDefinition type)
		{
			switch (type.TypeCode)
			{
				case TypeCode.Boolean:
					return value.ToString();
				case TypeCode.Char:
					return XmlConvert.ToString((ushort) (char) value);
				case TypeCode.SByte:
					return XmlConvert.ToString((sbyte) value);
				case TypeCode.Byte:
					return XmlConvert.ToString((byte) value);
				case TypeCode.Int16:
					return XmlConvert.ToString((short) value);
				case TypeCode.UInt16:
					return XmlConvert.ToString((ushort) value);
				case TypeCode.Int32:
					return XmlConvert.ToString((int) value);
				case TypeCode.UInt32:
					return XmlConvert.ToString((uint) value);
				case TypeCode.Int64:
					return XmlConvert.ToString((long) value);
				case TypeCode.UInt64:
					return XmlConvert.ToString((ulong) value);
				case TypeCode.Single:
					return XmlConvert.ToString((float) value);
				case TypeCode.Double:
					return XmlConvert.ToString((double) value);
				case TypeCode.Decimal:
					return XmlConvert.ToString((decimal) value);
				case TypeCode.DateTime:
					return XmlConvert.ToString((DateTime) value, XmlDateTimeSerializationMode.RoundtripKind);
				case TypeCode.String:
					return (string) value;
				default:
					if (type.Type == typeof(Guid))
					{
						return XmlConvert.ToString((Guid) value);
					}
					if (type.Type == typeof(TimeSpan))
					{
						return XmlConvert.ToString((TimeSpan) value);
					}
					return value.ToString();
			}
		}

		public static object GetPrimitiveValue(string value, TypeDefinition type, string nodeName)
		{
			try
			{
				if (type.IsEnum)
				{
					return Enum.Parse(type.Type, value);
				}

				switch (type.TypeCode)
				{
					case TypeCode.Boolean:
						return Convert.ToBoolean(value);
					case TypeCode.Char:
						return (char) XmlConvert.ToUInt16(value);
					case TypeCode.SByte:
						return XmlConvert.ToSByte(value);
					case TypeCode.Byte:
						return XmlConvert.ToByte(value);
					case TypeCode.Int16:
						return XmlConvert.ToInt16(value);
					case TypeCode.UInt16:
						return XmlConvert.ToUInt16(value);
					case TypeCode.Int32:
						return XmlConvert.ToInt32(value);
					case TypeCode.UInt32:
						return XmlConvert.ToUInt32(value);
					case TypeCode.Int64:
						return XmlConvert.ToInt64(value);
					case TypeCode.UInt64:
						return XmlConvert.ToUInt64(value);
					case TypeCode.Single:
						return XmlConvert.ToSingle(DecimalSeparator(value));
					case TypeCode.Double:
						return XmlConvert.ToDouble(DecimalSeparator(value));
					case TypeCode.Decimal:
						return XmlConvert.ToDecimal(DecimalSeparator(value));
					case TypeCode.DateTime:
						return XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.RoundtripKind);
					case TypeCode.String:
						return value;
					default:
						if (type.Type == typeof(Guid))
						{
							return XmlConvert.ToGuid(value);
						}
						if (type.Type == typeof(TimeSpan))
						{
							return XmlConvert.ToTimeSpan(value);
						}
						throw new NotSupportedException("Unknown primitive type " + type.Name + " - value: " + value);
				}
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(
					$"Unsuccessful conversion node {nodeName} for type {type.Name} - value {value}", ex);
			}
		}

		static string DecimalSeparator(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return value;
			}
			return value.Replace(",", ".");
		}
	}
}