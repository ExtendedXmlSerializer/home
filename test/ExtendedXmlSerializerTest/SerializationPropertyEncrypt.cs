using System.Collections.Generic;
using ExtendedXmlSerialization.Test.TestObject;
using Xunit;

namespace ExtendedXmlSerialization.Test
{
    public class SerializationPropertyEncrypt :BaseTest
    {
        public SerializationPropertyEncrypt()
        {
            Serializer.SerializationToolsFactory = new SimpleSerializationToolsFactory()
            {
                Configurations = new List<IExtendedXmlSerializerConfig> { new TestClassWithEncryptedDataConfig(), new TestClassWithEncryptedDataConfig() },
                EncryptionAlgorithm = new Base64PropertyEncryption()
            };
        }

        [Fact]
        public void SerializationRefernece()
        {
            TestClassWithEncryptedData obj = new TestClassWithEncryptedData();
            obj.Name = "Name";
            obj.Password = "Cxl2983Hd";
            obj.Salary = 100000;
            
            CheckSerializationAndDeserialization("ExtendedXmlSerializerTest.Resources.TestClassWithEncryptedData.xml", obj);
        }
    }
}
