using System;
using System.Text;

namespace ExtendedXmlSerialization.Samples.Encrypt
{
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