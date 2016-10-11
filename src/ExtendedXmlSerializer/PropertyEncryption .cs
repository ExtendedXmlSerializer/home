using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtendedXmlSerialization
{
    public interface IPropertyEncryption
    {
        string Encrypt(string value);
        string Decrypt(string value);
    }
}
