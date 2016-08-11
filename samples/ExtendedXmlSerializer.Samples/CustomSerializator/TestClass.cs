using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtendedXmlSerialization.Samples.CustomSerializator
{
    public class TestClass
    {
        public TestClass(string paramStr)
        {
            PropStr = paramStr;
        }

        public string PropStr { get; private set; }
    }
}
