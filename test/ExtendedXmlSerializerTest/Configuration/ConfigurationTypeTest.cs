using System;
using ExtendedXmlSerialization.Configuration;
using ExtendedXmlSerialization.Test.TestObject;
using Xunit;

namespace ExtendedXmlSerialization.Test.Configuration
{
    public class ConfigurationTypeTest
    {
        [Fact]
        public void TestClassWithEncryptedData()
        {
            Action<IExtendedXmlConfiguration> func =
                cfg =>
                {
                    cfg.ConfigureType<TestClassWithEncryptedData>()
                        .Property(p => p.Password).Encrypt()
                        .Property(p => p.Salary).Encrypt();

                    cfg.UseEncryptionAlgorithm(new Base64PropertyEncryption());
                };
            var configurer = new ExtendedXmlConfiguration();
            func(configurer);
            Assert.NotNull(configurer.EncryptionAlgorithm);
        }
    }
}
