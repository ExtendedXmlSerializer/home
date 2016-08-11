using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtendedXmlSerialization.Samples.Simple
{
    public class SimpleSamples
    {
        public static void Run()
        {
            ExtendedXmlSerializer serializer = new ExtendedXmlSerializer();
            Program.PrintHeader("Serialization");
            var obj = new TestClass();
            var xml = serializer.Serialize(obj);
            Console.WriteLine(xml);

            Program.PrintHeader("Deserialization");
            var obj2 = serializer.Deserialize<TestClass>(xml);
            Console.WriteLine("Obiect id = " + obj2.Id);

        }
    }
}
