using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue440Tests
	{
		[Fact]
		public void Verify()
		{
			var container = new ConfigurationContainer().Type<IInterface>()
			                                            .Register()
			                                            .Serializer()
			                                            .Using(Serializer.Default)
			                                            .Create()
			                                            .ForTesting();
			var instance = new Container {Interface = new Implementation()};
			container.Cycle(instance)
			         .Interface.Should()
			         .BeOfType<Implementation>()
			         .And.Subject.To<Implementation>()
			         .Created.Should()
			         .BeTrue();
		}

		sealed class Container
		{
			public IInterface Interface { get; set; }
		}

		sealed class Serializer : ISerializer<IInterface>
		{
			public static Serializer Default { get; } = new Serializer();

			Serializer() {}

			public IInterface Get(IFormatReader parameter)
			{
				var name   = parameter.Content();
				var type   = Type.GetType(name) ?? throw new InvalidOperationException($"Could not parse '{name}'");
				var result = (Implementation)Activator.CreateInstance(type);
				result.Created = true;
				return result;
			}

			public void Write(IFormatWriter writer, IInterface instance)
			{
				writer.Content(instance.GetType().AssemblyQualifiedName);
			}
		}

		public interface IInterface {}

		public sealed class Implementation : IInterface
		{
			public bool Created { get; set; }
		}
	}
}