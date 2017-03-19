// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
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

using System;
using ExtendedXmlSerialization.Configuration;

namespace ExtendedXmlSerialization.Samples.MigrationMap
{
	public class MigrationMapSamples
	{
		public static void RunSimpleConfig()
		{
			Program.PrintHeader("Deserialization old version of xml");

			var serializer =
				new ExtendedXmlConfiguration().Create(cfg => cfg.ConfigureType<TestClass>().AddMigration(new TestClassSerialiser()).Configuration);

			Run(serializer);
		}

//        public static void RunAutofacConfig()
//        {
//            Program.PrintHeader("Deserialization old version of xml - autofac config");
//
//            var builder = new ContainerBuilder();
//            builder.RegisterModule<AutofacExtendedXmlSerializerModule>();
//            builder.RegisterType<TestClassConfig>().As<ExtendedXmlSerializerConfig<TestClass>>().SingleInstance();
//            var containter = builder.Build();
//
//            var serializer = containter.Resolve<IExtendedXmlSerializer>();
//            Run(serializer);
//        }

		static void Run(IExtendedXmlSerializer serializer)
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