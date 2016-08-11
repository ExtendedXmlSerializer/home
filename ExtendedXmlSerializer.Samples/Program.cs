using System;
using ExtendedXmlSerialization.Samples.CustomSerializator;
using ExtendedXmlSerialization.Samples.MigrationMap;
using ExtendedXmlSerialization.Samples.Simple;

namespace ExtendedXmlSerialization.Samples
{
    public class Program
    {
        public static void PrintHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(title);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Main(string[] args)
        {
            SimpleSamples.Run();
            CustomSerializatorSamples.RunSimpleConfig();
            CustomSerializatorSamples.RunAutofacConfig();
            MigrationMapSamples.RunSimpleConfig();
            MigrationMapSamples.RunAutofacConfig();

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
