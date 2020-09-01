using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class PartitionedTypeSpecification : IPartitionedTypeSpecification
	{
		public static PartitionedTypeSpecification Default { get; } = new PartitionedTypeSpecification();

		PartitionedTypeSpecification() : this(typeof(object).Assembly) {}

		readonly Assembly _assembly;

		public PartitionedTypeSpecification(Assembly assembly) => _assembly = assembly;

		// HACK: This is a bit of a hack -- ok a total hack, to pass a test for .NET Framework under special conditions:
		// https://github.com/ExtendedXmlSerializer/home/issues/248
		public bool IsSatisfiedBy(TypeInfo parameter)
			=> parameter.Assembly != _assembly || (!parameter.Namespace?.Contains("System.Runtime.Remoting") ?? true);
	}
}