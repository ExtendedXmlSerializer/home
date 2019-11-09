using System.Reflection;
using System.Xml.Serialization;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	public sealed class MemberSpecification<T> : AllSpecification<T> where T : MemberInfo
	{
		readonly static AllowSpecification Specification = AllowSpecification.Default;

		public MemberSpecification(ISpecification<T> specification) : base(specification, Specification) {}

		sealed class AllowSpecification : InverseSpecification<MemberInfo>
		{
			public static AllowSpecification Default { get; } = new AllowSpecification();

			AllowSpecification() : base(new IsDefinedSpecification<XmlIgnoreAttribute>(false)) {}
		}
	}
}