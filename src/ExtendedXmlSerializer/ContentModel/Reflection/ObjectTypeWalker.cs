using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
// ReSharper disable TooManyDependencies

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class ObjectTypeWalker : InstanceMemberWalkerBase<TypeInfo>, ISource<IEnumerable<TypeInfo>>
	{
		readonly static VariableTypeMemberSpecifications Specifications = VariableTypeMemberSpecifications.Default;

		readonly IVariableTypeMemberSpecifications _specifications;
		readonly IMemberAccessors                  _accessors;

		public ObjectTypeWalker(ITypeMembers members, IEnumeratorStore enumerators, IMemberAccessors accessors,
		                        object root)
			: this(Specifications, accessors, members, enumerators, root) {}

		public ObjectTypeWalker(IVariableTypeMemberSpecifications specifications, IMemberAccessors accessors,
		                        ITypeMembers members, IEnumeratorStore enumerators, object root)
			: base(members, enumerators, root)
		{
			_specifications = specifications;
			_accessors      = accessors;
		}

		protected override IEnumerable<TypeInfo> Yield(IMember member, object instance)
		{
			var variable = _specifications.Get(member);
			if (variable != null)
			{
				var current = _accessors.Get(member)
				                        .Get(instance);
				if (Schedule(current) && variable.IsSatisfiedBy(current.GetType()))
				{
					yield return Defaults.FrameworkType;
				}
			}
		}

		protected override IEnumerable<TypeInfo> Yield(object instance)
		{
			Schedule(instance);
			yield break;
		}

		protected override IEnumerable<TypeInfo> Members(object input, TypeInfo parameter)
			=> parameter.Yield()
			            .Concat(base.Members(input, parameter));

		public IEnumerable<TypeInfo> Get() => this.SelectMany(x => x)
		                                          .Distinct();
	}
}