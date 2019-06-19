using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedXmlSerializer.Samples.Parametrized
{
    public static class ParametrizedConstructors
    {
        public static void SerializeAndDeserialize()
        {
            var srl = new ConfigurationContainer()
                                            .EnableAllPublicPropertiesAndParameterizedContent()
                                            .Create();

            var dsrl = new ConfigurationContainer()
                                            .EnableAllPublicPropertiesAndParameterizedContent()
                                            .Create();

            var dt = new DataHolder(11);
            dt.Name = "my super holder";
            dt.Data.Add(3);
            dt.Data.Add(5);
            dt.Data.Add(7);

            var data = srl.Serialize(dt);
            var obj = dsrl.Deserialize<DataHolder>(data);
        }

        public class DataHolder
        {
            public DataHolder(int lenght)
            {
                Lenght = lenght;
                Data = new List<int>();
            }

            public int Lenght { get; }
            public List<int> Data { get; set; }
            public string Name { get; set; }
        }
    }

    
}
