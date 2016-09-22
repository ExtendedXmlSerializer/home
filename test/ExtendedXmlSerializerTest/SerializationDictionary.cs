using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtendedXmlSerialization.Test.TestObject;
using ExtendedXmlSerialization.Test.TestObjectConfigs;
using Xunit;

namespace ExtendedXmlSerialization.Test
{
    public class SerializationDictionary :BaseTest
    {
        public class TestClass
        {
            public Dictionary<int, string> Dictionary { get; set; }
        }
        [Fact]
        public void DictionaryOfIntAndString()
        {
            var dict = new Dictionary<int, string>();
            dict.Add(1,"First");
            dict.Add(2,"Second");
            dict.Add(3,"Other");

            CheckSerializationAndDeserializationByXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
<ArrayOfInt32String>
    <Item>
        <Key>1</Key>
        <Value>First</Value>
    </Item>
    <Item>
        <Key>2</Key>
        <Value>Second</Value>
    </Item>
    <Item>
        <Key>3</Key>
        <Value>Other</Value>
    </Item>
</ArrayOfInt32String>", dict);
        }

        [Fact]
        public void PropertyDictionary()
        {
            var obj = new TestClass
            {
                Dictionary = new Dictionary<int, string>
                {
                    {1, "First"},
                    {2, "Second"},
                    {3, "Other"},

                }
            };

            CheckSerializationAndDeserializationByXml(@"<TestClass type=""ExtendedXmlSerialization.Test.SerializationDictionary+TestClass"">
  <Dictionary>
    <Item>
        <Key>1</Key>
        <Value>First</Value>
    </Item>
    <Item>
        <Key>2</Key>
        <Value>Second</Value>
    </Item>
    <Item>
        <Key>3</Key>
        <Value>Other</Value>
    </Item>
  </Dictionary>
</TestClass>", obj);
        }
    }
}
