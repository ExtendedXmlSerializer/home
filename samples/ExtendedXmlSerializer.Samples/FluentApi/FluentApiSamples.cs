using System;
using System.Collections.Generic;
using System.Text;

namespace ExtendedXmlSerializer.Samples.FluentApi
{
	using System.Linq;

	using ExtendedXmlSerializer.Configuration;
	using ExtendedXmlSerializer.ExtensionModel.Encryption;
	using ExtendedXmlSerializer.ExtensionModel.Xml;
	using ExtendedXmlSerializer.Samples.CustomSerializator;
	using ExtendedXmlSerializer.Samples.Encrypt;

	public class FluentApiSamples
    {
	    public static void RunSimpleConfig()
	    {
		    Program.PrintHeader("Serialization reference object");

		    // Configuration

//		    var serializer = new ConfigurationContainer()
//				.UseEncryptionAlgorithm(new CustomEncryption())
//			    .Type<Person>()
//					.Member(p => p.Password).Name("Pwd")
//			    .Type<Person>()

//				.Create();
		    // EndConfiguration

//		    Run(serializer);
	    }

	    //        public static void RunAutofacConfig()
	    //        {
	    //            Program.PrintHeader("Serialization reference object - autofac config");
	    //
	    //            var builder = new ContainerBuilder();
	    //            builder.RegisterModule<AutofacExtendedXmlSerializerModule>();
	    //            builder.RegisterType<PersonConfig>().As<ExtendedXmlSerializerConfig<Person>>().SingleInstance();
	    //            builder.RegisterType<Base64PropertyEncryption>().As<IPropertyEncryption>().SingleInstance();
	    //            var containter = builder.Build();
	    //
	    //            var serializer = containter.Resolve<IExtendedXmlSerializer>();
	    //            Run(serializer);
	    //        }

	    static void Run(IExtendedXmlSerializer serializer)
	    {
		    var list = new List<Person>
			               {
				               new Person {Name = "John", Password = "Ab238ds2"},
				               new Person {Name = "Oliver", Password = "df89nmXhdf"}
			               };

		    var xml = serializer.Serialize(list);
		    Console.WriteLine(xml);

		    var obj2 = serializer.Deserialize<List<Person>>(xml);
		    Console.WriteLine("Employees count = " + obj2.Count + " - passwords " +
		                      string.Join(", ", obj2.Select(p => p.Password)));
	    }
    }
}

