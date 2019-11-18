using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System.Reflection;
using System.Xml.Serialization;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	/// <summary>
	/// A base component that is used for metadata specifications.
	/// </summary>
	/// <typeparam name="T">The member metadata type.</typeparam>
	public sealed class MemberSpecification<T> : AllSpecification<T> where T : MemberInfo
	{
		readonly static AllowSpecification Specification = AllowSpecification.Default;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="specification">A supplemental specification.</param>
		public MemberSpecification(ISpecification<T> specification) : base(specification, Specification) {}

		sealed class AllowSpecification : InverseSpecification<MemberInfo>
		{
			public static AllowSpecification Default { get; } = new AllowSpecification();

			AllowSpecification() : base(new IsDefinedSpecification<XmlIgnoreAttribute>(false)) {}
		}
	}
}