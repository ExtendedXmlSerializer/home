using ExtendedXmlSerializer.Configuration;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.Samples.Parametrized
{
    public static class ParametrizedConstructors
    {
        public static void SerializeAndDeserialize()
        {
            var srl = new ConfigurationContainer()
                                            .EnableParameterizedContentWithPropertyAssignments()
                                            .Create();

            var dsrl = new ConfigurationContainer()
                                            .EnableParameterizedContentWithPropertyAssignments()
                                            .Create();

            var dt = new DataHolder(11);
            dt.Name = "my super holder";
            dt.Data.Add(3);
            dt.Data.Add(5);
            dt.Data.Add(7);

            var data = srl.Serialize(dt);
            dsrl.Deserialize<DataHolder>(data);
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
