using System;
using System.Text;

namespace ExtendedXmlSerialization.Test.TestObject
{
    public class TestClassWithEncryptedData
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public decimal Salary { get; set; }
    }

    public class Base64PropertyEncryption : IPropertyEncryption
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
