using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using ExtendedXmlSerialization.Test.TestObject;
using Xunit;

namespace ExtendedXmlSerialization.Test
{
    public class SerializationStruct :BaseTest
    {
        [Fact]
        public void Struct()
        {
            Vector2 vector2 = new Vector2(1, 2);

            CheckSerializationAndDeserializationByXml(
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<Vector2 type=""System.Numerics.Vector2""><X>1</X><Y>2</Y></Vector2>",
                vector2);
            CheckCompatibilityWithDefaultSerializator(vector2);
            
        }

        [Fact]
        public void StructWithConst()
        {
            var  obj  = new TestStruct();
            obj.Init();
            CheckSerializationAndDeserializationByXml(
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<TestStruct type=""ExtendedXmlSerialization.Test.TestObject.TestStruct""><A>1</A><B>2</B></TestStruct>",
                obj);
            CheckCompatibilityWithDefaultSerializator(obj);
        }
    }
}
