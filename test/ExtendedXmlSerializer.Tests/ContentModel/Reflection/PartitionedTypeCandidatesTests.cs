using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.ReflectionModel;
using System.Reflection;
using Xunit;
using Identity = ExtendedXmlSerializer.ContentModel.Identification.Identity;

namespace ExtendedXmlSerializer.Tests.ContentModel.Reflection
{
	public class PartitionedTypeCandidatesTests
	{
		[Fact]
		public void TestName()
		{
			var expected   = typeof(Subject).GetTypeInfo();
			var @namespace = NamespaceFormatter.Default.Get(expected);
			var aliases    = new Names(new TypedTable<string>(DefaultNames.Default));
			var formatter  = new TypeFormatter(aliases);
			var partitions = new AssemblyTypePartitions(PartitionedTypeSpecification.Default, formatter);
			var type = new PartitionedTypeCandidates(TypeLoader.Default, partitions)
			           .Get(new Identity(formatter.Get(expected), @namespace))
			           .Only();
			Assert.Equal(expected, type);
		}

		sealed class Subject {}
	}
}