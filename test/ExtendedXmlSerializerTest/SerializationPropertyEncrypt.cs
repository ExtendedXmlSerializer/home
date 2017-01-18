using System.Collections.Generic;
using ExtendedXmlSerialization.Test.TestObject;
using Xunit;

namespace ExtendedXmlSerialization.Test
{
    public class SerializationPropertyEncrypt :BaseTest
    {
        public SerializationPropertyEncrypt()
        {
            Serializer = new ExtendedXmlSerializer(cfg =>
            {
                cfg.ConfigType<TestClassWithEncryptedData>()
                    .Property(p=>p.Password).Encrypt()
                    .Property(p=>p.Salary).Encrypt();

                cfg.EncryptionAlgorithm = new Base64PropertyEncryption();
            });
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
