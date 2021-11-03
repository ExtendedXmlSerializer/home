using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Linq;
using System.Text;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue565Tests
	{
		[Fact]
		public void VerifyExclude()
		{
			var subject = new ConfigurationContainer().EnableParameterizedContent()
			                                          .Type<Model>()
			                                          .EmitWhen(x => x.Entity is not Ignored)
			                                          .Create()
			                                          .ForTesting();
			var instance = new[]
			{
				new Model("First", new Implementation1()),
				new Model("Ignored", new Ignored()),
				new Model("Second", new Implementation2())
			};

			var cycled = subject.Cycle(instance);

			// If you want to use dictionary in memory:
			var index = instance.ToDictionary(x => x.Key);
			cycled.Should().HaveCount(2);
			index["First"].Entity.Should().BeOfType<Implementation1>();
			index["First"].Data.Should().NotBeNullOrEmpty();
			index["Second"].Entity.Should().BeOfType<Implementation2>();
			index["Second"].Data.Should().NotBeNullOrEmpty();
		}

		sealed class Model
		{
			public Model(byte[] data, IDispenseEntity entity) : this(Encoding.UTF8.GetString(data), data, entity) {}

			public Model(string key, IDispenseEntity entity) : this(key, Encoding.UTF8.GetBytes(key), entity) {}

			public Model(string key, byte[] data, IDispenseEntity entity)
			{
				Key    = key;
				Data   = data;
				Entity = entity;
			}

			public string Key { get; }
			public byte[] Data { get; }
			public IDispenseEntity Entity { get; }
		}

		interface IDispenseEntity {}

		sealed class Implementation1 : IDispenseEntity {}

		sealed class Implementation2 : IDispenseEntity {}

		sealed class Ignored : IDispenseEntity {}
	}
}
