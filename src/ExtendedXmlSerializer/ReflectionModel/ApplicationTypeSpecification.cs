using ExtendedXmlSerializer.Core.Specifications;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class ApplicationTypeSpecification : InverseSpecification<TypeInfo>
	{
		public static ApplicationTypeSpecification Default { get; } = new ApplicationTypeSpecification();

		ApplicationTypeSpecification()
			: base(IsDefinedSpecification<CompilerGeneratedAttribute>.Default.Or(IsUnspeakable.Default)) {}
	}
}