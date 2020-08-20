using ExtendedXmlSerializer.Core.Specifications;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class PartitionedTypeSpecification
		: DecoratedSpecification<TypeInfo>,
		  IPartitionedTypeSpecification
	{
		public static PartitionedTypeSpecification Default { get; } = new PartitionedTypeSpecification();

		PartitionedTypeSpecification() : base(NotHappy.Default) {}
	}

	sealed class NotHappy : ISpecification<TypeInfo>
		// HACK: This is a bit of a hack -- ok a total hack, to pass a test for .NET Framework under special conditions: https://github.com/ExtendedXmlSerializer/home/issues/248
		// This doesn't occur for .NET Core, but we will have to do better for v3 in any case.
	{
		public static NotHappy Default { get; } = new NotHappy();

		NotHappy() : this(typeof(object).Assembly) {}

		readonly Assembly _assembly;

		public NotHappy(Assembly assembly) => _assembly = assembly;

		public bool IsSatisfiedBy(TypeInfo parameter)
			=> parameter.Assembly != _assembly
			   ||
			   (!parameter.Namespace?.Contains("System.Runtime.Remoting") ?? false);
	}
}