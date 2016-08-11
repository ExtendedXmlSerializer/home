using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using ExtendedXmlSerialization.Autofac;

namespace ExtendedXmlSerialization.Samples.CustomSerializator
{
    public class CustomSerializatorSamples
    {
        public static void RunSimpleConfig()
        {
            Program.PrintHeader("Custom serialization");

            var toolsFactory = new SimpleSerializationToolsFactory();
            toolsFactory.CustomSerializators.Add(new TestClassSerializer());
            ExtendedXmlSerializer serializer = new ExtendedXmlSerializer(toolsFactory);

            Run(serializer);
        }

        public static void RunAutofacConfig()
        {
            Program.PrintHeader("Custom serialization - autofac config");

            var builder = new ContainerBuilder();
            builder.RegisterModule<AutofacExtendedXmlSerializerModule>();
            builder.RegisterType<TestClassSerializer>().As<ICustomSerializator<TestClass>>().SingleInstance();
            var containter = builder.Build();

            var serializer = containter.Resolve<IExtendedXmlSerializer>();
            Run(serializer);
        }

        private static void Run(IExtendedXmlSerializer serializer)
        {
            var obj = new TestClass("Value");
            var xml = serializer.Serialize(obj);
            Console.WriteLine(xml);

            var obj2 = serializer.Deserialize<TestClass>(xml);
            Console.WriteLine("Obiect PropStr = " + obj2.PropStr);
        }
    }
}
