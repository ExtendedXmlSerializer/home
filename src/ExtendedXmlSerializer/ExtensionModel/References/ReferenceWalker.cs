using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class ReferenceWalker : InstanceMemberWalkerBase<object>, ISource<ImmutableArray<object>>
	{
		readonly IReferencesPolicy _policy;
		readonly IMemberAccessors  _accessors;

		// ReSharper disable once TooManyDependencies
		public ReferenceWalker(IReferencesPolicy policy, ITypeMembers members, IEnumeratorStore enumerators,
		                       IMemberAccessors accessors, object root)
			: base(members, enumerators, root)
		{
			_policy    = policy;
			_accessors = accessors;
		}

		protected override IEnumerable<object> Yield(IMember member, object instance)
			=> Yield(_accessors.Get(member)
			                   .Get(instance));

		protected override IEnumerable<object> Yield(object instance)
		{
			if (!Schedule(instance) && Check(instance))
			{
				yield return instance;
			}
		}

		bool Check(object instance)
		{
			var info = instance?.GetType()
			                   .GetTypeInfo();
			var check = info != null && !info.IsValueType && _policy.IsSatisfiedBy(info);
			return check;
		}

		public ImmutableArray<object> Get() => this.SelectMany(x => x)
		                                           .Distinct()
		                                           .ToImmutableArray();
	}
}