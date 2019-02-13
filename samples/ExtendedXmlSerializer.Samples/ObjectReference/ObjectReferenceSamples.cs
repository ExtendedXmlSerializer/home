// MIT License
//
// Copyright (c) 2016-2018 Wojciech Nagórski
//                    Michael DeMond
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ExtendedXmlSerializer.Samples.ObjectReference
{
	public class ObjectReferenceSamples
	{
		public static void RunSimpleConfig()
		{
			Program.PrintHeader("Serialization reference object");

// Configure

IExtendedXmlSerializer serializer = new ConfigurationContainer().ConfigureType<Person>()
                                             .EnableReferences(p => p.Id)
                                             .Create();
// EndConfigure
            Run(serializer);
		}

//        public static void RunAutofacConfig()
//        {
//            Program.PrintHeader("Serialization reference object - autofac config");
//
//            var builder = new ContainerBuilder();
//            builder.RegisterModule<AutofacExtendedXmlSerializerModule>();
//            builder.RegisterType<PersonConfig>().As<ExtendedXmlSerializerConfig<Person>>().SingleInstance();
//            var containter = builder.Build();
//
//            var serializer = containter.Resolve<IExtendedXmlSerializer>();
//            Run(serializer);
//        }

		static void Run(IExtendedXmlSerializer serializer)
		{
// CreateObject

Person boss = new Person {Id = 1, Name = "John"};
boss.Boss = boss; //himself boss
Person worker = new Person {Id = 2, Name = "Oliver"};
worker.Boss = boss;
Company obj = new Company
{
	Employees = new List<Person>
	{
		worker,
		boss
	}
};
// EndCreateObject
		    string xml = serializer.Serialize(new XmlWriterSettings {Indent = true}, obj);

            File.WriteAllText("bin\\ObjectReferenceSamples.xml", xml);
            Console.WriteLine(xml);

			Company obj2 = serializer.Deserialize<Company>(xml);
			Console.WriteLine("Employees count = " + obj2.Employees.Count);
		}
	}
}