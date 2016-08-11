using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using ExtendedXmlSerialization.Autofac;

namespace ExtendedXmlSerialization.Samples.MigrationMap
{
    public class MigrationMapSamples
    {
        public static void RunSimpleConfig()
        {
            Program.PrintHeader("Deserialization old version of xml");
            var toolsFactory = new SimpleSerializationToolsFactory();
            toolsFactory.MigrationMaps.Add(new TestClassMigrationMap());
            ExtendedXmlSerializer serializer = new ExtendedXmlSerializer(toolsFactory);

            Run(serializer);
        }        
        public static void RunAutofacConfig()
        {
            Program.PrintHeader("Deserialization old version of xml - autofac config");

            var builder = new ContainerBuilder();
            builder.RegisterModule<AutofacExtendedXmlSerializerModule>();
            builder.RegisterType<TestClassMigrationMap>().As<IMigrationMap<TestClass>>().SingleInstance();
            var containter = builder.Build();

            var serializer = containter.Resolve<IExtendedXmlSerializer>();
            Run(serializer);
        }

        private static void Run(IExtendedXmlSerializer serializer)
        {
            var xml =
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<TestClass type=""ExtendedXmlSerialization.Samples.MigrationMap.TestClass"">
<Id>1</Id>
<Type>Type</Type>
</TestClass>";
            Console.WriteLine(xml);
            var obj = serializer.Deserialize<TestClass>(xml);

            Console.WriteLine("Obiect Id = " + obj.Id);
            Console.WriteLine("Obiect Name = " + obj.Name);
            Console.WriteLine("Obiect Value = " + obj.Value);

            Console.WriteLine("Serialization to new version");
            var xml2 = serializer.Serialize(obj);
            Console.WriteLine(xml2);
        }
    }
}
