// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
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
using System.Collections.Generic;
using ExtendedXmlSerialization.Test.TestObject;
using ExtendedXmlSerialization.Test.Tools;
using Xunit;

namespace ExtendedXmlSerialization.Test
{
    public class SerializatorMigrationMapTest : BaseTest
    {
        public SerializatorMigrationMapTest()
        {
            Serializer.SerializationToolsFactory = new SimpleSerializationToolsFactory()
            {
                MigrationMaps = new List<IMigrationMap> { new TestClassMigrationMap() }
            };
        }

        [Fact]
        public void MapVersion2()
        {
            var xml = EmbeddedResources.Get("ExtendedXmlSerializerTest.Resources.TestClassWithMapV2.xml");
            var klasa = Serializer.Deserialize<TestClassWithMap>(xml);

            Assert.Equal(klasa.NowyWezel, "test");
            Assert.Equal(klasa.ZmianaWartosci, "Stara");
            Assert.NotNull(klasa.PropClass);
            Assert.Equal(klasa.PropClass.Wartosc, 12);
            Assert.Equal(klasa.PropClass.Wezel1, "WartoscWezlas");
        }

        [Fact]
        public void MapVersion1()
        {
            var xml = EmbeddedResources.Get("ExtendedXmlSerializerTest.Resources.TestClassWithMapV1.xml");

            var klasa = Serializer.Deserialize<TestClassWithMap>(xml);
            Assert.Equal(klasa.NowyWezel, "test");
            Assert.Equal(klasa.ZmianaWartosci, "Stara");
            Assert.NotNull(klasa.PropClass);
            Assert.Equal(klasa.PropClass.Wartosc, 1);
            Assert.Equal(klasa.PropClass.Wezel1, "WartoscInna");
        }

        [Fact]
        public void MapVersion0()
        {
            var xml = EmbeddedResources.Get("ExtendedXmlSerializerTest.Resources.TestClassWithMapV0.xml");

            var klasa = Serializer.Deserialize<TestClassWithMap>(xml);
            Assert.Equal(klasa.NowyWezel, "Wartosc");
            Assert.Equal(klasa.ZmianaWartosci, "Nowa");
            Assert.NotNull(klasa.PropClass);
            Assert.Equal(klasa.PropClass.Wartosc, 1);
            Assert.Equal(klasa.PropClass.Wezel1, "WartoscInna");

        }
        [Fact]
        public void MapVersionXmlWithOutVersion()
        {
            var xml = EmbeddedResources.Get("ExtendedXmlSerializerTest.Resources.TestClassWithMapWithoutVer.xml");

            var klasa = Serializer.Deserialize<TestClassWithMap>(xml);
            Assert.Equal(klasa.NowyWezel, "Wartosc");
            Assert.Equal(klasa.ZmianaWartosci, "Nowa");
            Assert.NotNull(klasa.PropClass);
            Assert.Equal(klasa.PropClass.Wartosc, 1);
            Assert.Equal(klasa.PropClass.Wezel1, "WartoscInna");

        }
//TODO XUNIT
//        [Fact]
//        [ExpectedException("System.Xml.XmlException", ExpectedMessage = "DeserializatorXml: Nieznany numer wersji 99 dla typu ExtendedXmlSerialization.Test.TestObject.TestClassWithMap.")]
//        public void MapVersion99Exception()
//        {
//            var xml = EmbeddedResources.Get("ExtendedXmlSerializerTest.Resources.TestClassWithMapV99.xml");
//            ExtendedXmlSerializer.Deserialize<TestClassWithMap>(xml);
//        }
    }
}
