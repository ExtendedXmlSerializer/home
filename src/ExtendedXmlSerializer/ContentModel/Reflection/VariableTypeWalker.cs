using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	class VariableTypeWalker : TypeMemberWalkerBase<TypeInfo>, ISource<IEnumerable<TypeInfo>>
	{
		readonly static ISpecification<TypeInfo> Specification =
			ReflectionModel.VariableTypeSpecification.Default.Or(IsArraySpecification.Default);

		readonly ISpecification<TypeInfo> _specification;

		public VariableTypeWalker(ITypeMembers members, TypeInfo root) : this(Specification, members, root) {}

		public VariableTypeWalker(ISpecification<TypeInfo> specification, ITypeMembers members, TypeInfo root)
			: base(members, root) => _specification = specification;

		protected override IEnumerable<TypeInfo> Select(TypeInfo type)
		{
			foreach (var typeInfo in type.Yield()
			                             .Concat(base.Select(type)))
			{
				if (_specification.IsSatisfiedBy(typeInfo))
				{
					yield return typeInfo;
				}
			}
		}

		protected override IEnumerable<TypeInfo> Yield(IMember member)
		{
			var type = member.MemberType;
			if (!Schedule(type))
			{
				yield return type;
			}
		}

		public IEnumerable<TypeInfo> Get() => this.SelectMany(x => x);
	}
}