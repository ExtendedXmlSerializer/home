using ExtendedXmlSerializer.Core.Specifications;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	/// <summary>
	/// Reference: https://stackoverflow.com/a/9256695/10340424
	/// </summary>
	sealed class IsUnspeakable : DelegatedSpecification<TypeInfo>
	{
		public static IsUnspeakable Default { get; } = new IsUnspeakable();

		IsUnspeakable() : base(x => x.Name.StartsWith("<")) {}
	}
}