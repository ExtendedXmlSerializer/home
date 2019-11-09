using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class IsSerializableMember : ISpecification<MemberInfo>
	{
		public static IsSerializableMember Default { get; } = new IsSerializableMember();

		IsSerializableMember() {}

		public bool IsSatisfiedBy(MemberInfo parameter)
		{
			switch (parameter.MemberType)
			{
				case MemberTypes.Property:
				case MemberTypes.Field:
				case MemberTypes.TypeInfo:
					return true;
				default:
					return false;
			}
		}
	}
}