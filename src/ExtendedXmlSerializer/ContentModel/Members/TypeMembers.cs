using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class TypeMembers : CacheBase<TypeInfo, ImmutableArray<IMember>>, ITypeMembers
	{
		readonly Func<IMember, bool> _specification;
		readonly ITypeMemberSource   _source;

		[UsedImplicitly]
		public TypeMembers(IValidMemberSpecification specification, ITypeMemberSource source)
			: this(specification.IsSatisfiedBy, source) {}

		public TypeMembers(Func<IMember, bool> specification, ITypeMemberSource source)
		{
			_specification = specification;
			_source        = source;
		}

		protected override ImmutableArray<IMember> Create(TypeInfo parameter)
		{
			var result = _source.Get(parameter)
			                    .Where(_specification)
			                    .OrderBy(x => x.Order)
			                    .ToImmutableArray();
			return result;
		}
	}
}