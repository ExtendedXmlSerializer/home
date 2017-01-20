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
using System.Text;
using ExtendedXmlSerialization.Legacy;
#pragma warning disable 618

namespace ExtendedXmlSerialization.Test.TestObject
{
	public class TestClassWithEncryptedData
	{
		public string Name { get; set; }
		public string Password { get; set; }
		public decimal Salary { get; set; }
	}

	public class TestClassWithEncryptedDataConfig : ExtendedXmlSerializerConfig<TestClassWithEncryptedData>
	{
		public TestClassWithEncryptedDataConfig()
		{
			Encrypt(p => p.Password);
			Encrypt(p => p.Salary);
		}
	}

	public class Base64PropertyEncryption : IPropertyEncryption, ExtendedXmlSerialization.Legacy.IPropertyEncryption
	{
		public string Encrypt(string value)
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
		}

		public string Decrypt(string value)
		{
			return Encoding.UTF8.GetString(Convert.FromBase64String(value));
		}
	}
}