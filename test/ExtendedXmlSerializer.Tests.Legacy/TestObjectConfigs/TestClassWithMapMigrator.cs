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
using System.Xml.Linq;
using ExtendedXmlSerialization;
using ExtendedXmlSerializer.Tests.Legacy.TestObject;


#pragma warning disable 618

namespace ExtendedXmlSerializer.Tests.Legacy.TestObjectConfigs
{
	public class TestClassWithMapMigrator : ExtendedXmlSerializerConfig<TestClassWithMap>, IExtendedXmlTypeMigrator
	{
		public TestClassWithMapMigrator()
		{
			AddMigration(MigrationV0).AddMigration(MigrationV1);
		}

		public static void MigrationV0(XElement node)
		{
			// Delete not exists node
			node.Elements().Where(x => x.Name == "NieistniejacyWezel").Remove();
			// Delete unused node
			node.Elements().Where(x => x.Name == "NieprzydatnyWezel").Remove();
			// Add new node
			node.Add(new XElement("NowyWezel", "Wartosc"));
			// Change value
			var element = node.Element("ZmianaWartosci");
			if (element != null)
			{
				element.Value = "Nowa";
			}
		}

		public static void MigrationV1(XElement node)
		{
			var element = node.Element("MapToClass");
			if (element != null)
			{
				var newElement = new XElement("PropClass", new XElement("Wezel1", element.Value),
											  new XElement("Wartosc", "1"));
				newElement.SetAttributeValue("type", "ExtendedXmlSerializer.Tests.Legacy.TestObject.TestClassMapFromPrimitive");
				node.Add(newElement);
				element.Remove();
			}
		}

		public IEnumerable<Action<XElement>> GetAllMigrations()
		{
			yield return MigrationV0;
			yield return MigrationV1;
		}
	}
}