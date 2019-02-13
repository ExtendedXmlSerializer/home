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
using ExtendedXmlSerializer.ExtensionModel.Encryption;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtendedXmlSerializer.Samples.Encrypt
{
	public class EncryptSamples
	{
		public static void RunSimpleConfig()
		{
			Program.PrintHeader("Serialization reference object");

// Configuration

IExtendedXmlSerializer serializer = new ConfigurationContainer().UseEncryptionAlgorithm(new CustomEncryption())
                                             .ConfigureType<Person>()
                                             .Member(p => p.Password)
                                             .Encrypt()
                                             .Create();
// EndConfiguration

			Run(serializer);
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
			List<Person> list = new List<Person>
					   {
						   new Person {Name = "John", Password = "Ab238ds2"},
						   new Person {Name = "Oliver", Password = "df89nmXhdf"}
					   };

			string xml = serializer.Serialize(list);
			Console.WriteLine(xml);

			List<Person> obj2 = serializer.Deserialize<List<Person>>(xml);
			Console.WriteLine("Employees count = " + obj2.Count + " - passwords " +
							  string.Join(", ", obj2.Select(p => p.Password)));
		}
	}


// CustomEncryption

public class CustomEncryption : IEncryption
{
	public string Parse(string data)
		=> Encoding.UTF8.GetString(Convert.FromBase64String(data));

	public string Format(string instance)
		=> Convert.ToBase64String(Encoding.UTF8.GetBytes(instance));
}
// EndCustomEncryption
}