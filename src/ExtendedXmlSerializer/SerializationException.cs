using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtendedXmlSerialization
{
    public class SerializationException : Exception
    {
        public SerializationException(string message) : base(message) {}
        public SerializationException(string message, Exception innerException) : base(message, innerException) {}
    }
}
