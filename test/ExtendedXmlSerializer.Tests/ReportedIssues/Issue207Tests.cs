using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue207Tests
	{
		[Fact]
		void Verify()
		{
			var support = new ConfigurationContainer().EnableAllPublicPropertiesAndParameterizedContent()
			                                          .Create()
			                                          .ForTesting();

            var instance = new DataHolder(13);
            instance.Lenght = 13;
            instance.Data.Add(new Data(1.23f));
            instance.Data.Add(new Data(3.27f));
            instance.Data.Add(new Data(5.14f));
            instance.Data.Add(new Data(7.29f));

            support.Cycle(instance).ShouldBeEquivalentTo(instance);
		}

		public class DataHolder
		{
            public DataHolder(int value)
            {
                Value = value;
                Data = new List<Data>();
            }

			public int Value { get; set; } = 5;
            public double Lenght { get; set; } = 1.57;
            public List<Data> Data { get; }
        }

        public class Data
        {
            public Data(float d)
            {
                D = d;
            }

            public float D { get; set; }
        }
	}
}