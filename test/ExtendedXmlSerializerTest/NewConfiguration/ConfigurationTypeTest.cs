using System;
using ExtendedXmlSerialization.Configuration;
using ExtendedXmlSerialization.Test.TestObject;
using Xunit;

namespace ExtendedXmlSerialization.Test.NewConfiguration
{
    public class ConfigurationTypeTest
    {
        [Fact]
        public void TestClassWithEncryptedData()
        {
            Action<IExtendedXmlSerializerConfig> func =
                cfg =>
                {
                    cfg.ConfigType<TestClassWithEncryptedData>()
                        .Property(p => p.Password).Encrypt()
                        .Property(p => p.Salary).Encrypt();

                    cfg.UseEncryptionAlgorithm(new Base64PropertyEncryption());
                };
            var configurer = new ExtendedXmlSerializerConfig();
            func(configurer);
            Assert.NotNull(configurer.EncryptionAlgorithm);
        }
    }
}
