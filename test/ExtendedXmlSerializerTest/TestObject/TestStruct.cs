using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtendedXmlSerialization.Test.TestObject
{
    public struct TestStruct
    {
        public void Init()
        {
            A = 1;
            B = 2;

        }

        public int A;
        public int B { get; set; }

        public const int C = 3;
    }
}
