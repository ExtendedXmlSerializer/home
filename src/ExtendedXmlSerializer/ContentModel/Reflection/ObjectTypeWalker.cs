using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable TooManyDependencies

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class ObjectTypeWalker : InstanceMemberWalkerBase<Type>, ISource<IEnumerable<Type>>
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

		protected override IEnumerable<Type> Yield(IMember member, object instance)
		{
			var current = _accessors.Get(member).Get(instance);
			if (current != null)
			{
				var variable = _specifications.Get(member);
				if (variable != null)
				{
					if (!Schedule(current) || variable.IsSatisfiedBy(current.GetType()))
					{
						yield return Defaults.FrameworkType;
					}
				}
				else if (!First(current))
				{
					yield return Defaults.FrameworkType;
				}
			}
		}

		protected override IEnumerable<Type> Yield(object instance)
		{
			if (!Schedule(instance))
			{
				yield return Defaults.FrameworkType;
			}
		}

		protected override IEnumerable<Type> Members(object input, Type parameter)
			=> parameter.Yield().Concat(base.Members(input, parameter));

		public IEnumerable<Type> Get() => this.SelectMany(x => x).Distinct();
	}
}