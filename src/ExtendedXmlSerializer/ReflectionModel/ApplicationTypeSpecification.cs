using System.Reflection;
using System.Runtime.CompilerServices;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ReflectionModel
{
	class ApplicationTypeSpecification : InverseSpecification<TypeInfo>
	{
		public static ApplicationTypeSpecification Default { get; } = new ApplicationTypeSpecification();

		ApplicationTypeSpecification() : base(IsDefinedSpecification<CompilerGeneratedAttribute>.Default) {}
	}
}