using BenchmarkDotNet.Running;

namespace ExtendedXmlSerialization.Performance.Tests
{
    class Program
    {
        // ReSharper disable once UnusedMember.Local
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
