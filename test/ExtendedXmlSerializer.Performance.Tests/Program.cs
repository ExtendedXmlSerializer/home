using BenchmarkDotNet.Running;

namespace ExtendedXmlSerialization.Performance.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var switcher = new BenchmarkSwitcher(new[]
            {
                typeof(ExtendedXmlSerializerTest),
                typeof(ExtendedXmlSerializerV2Test),
                typeof(XmlSerializerTest)
            });
            switcher.Run(args);
        }
    }
}
