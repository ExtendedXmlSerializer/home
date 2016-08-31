using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtendedXmlSerialization.Test.TestObject;
using Xunit;

namespace ExtendedXmlSerialization.Test
{
    public class SerializationDifferentProperty:BaseTest
    {
        [Fact]
        public void DifferentProperty()
        {
            var obj = new TestClassPropertyType();
            obj.Init();

            this.CheckCompatibilityWithDefaultSerializator(obj);

    }
    }
}
