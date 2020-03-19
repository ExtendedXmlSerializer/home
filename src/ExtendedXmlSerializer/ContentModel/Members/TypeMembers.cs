using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using JetBrains.Annotations;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class TypeMembers : CacheBase<TypeInfo, ImmutableArray<IMember>>, ITypeMembers
	{
		readonly IContainsCustomSerialization _custom;
		readonly Func<IMember, bool>          _specification;
		readonly ITypeMemberSource            _source;

		[UsedImplicitly]
		public TypeMembers(IValidMemberSpecification specification, IContainsCustomSerialization custom,
		                   ITypeMemberSource source) : this(custom, specification.IsSatisfiedBy, source) {}

		public TypeMembers(IContainsCustomSerialization custom, Func<IMember, bool> specification,
		                   ITypeMemberSource source)
		{
			_custom        = custom;
			_specification = specification;
			_source        = source;
		}

		protected override ImmutableArray<IMember> Create(TypeInfo parameter)
		{
			var result = _custom.IsSatisfiedBy(parameter)
				             ? ImmutableArray<IMember>.Empty
				             : _source.Get(parameter)
				                      .Where(_specification)
				                      .OrderBy(x => x.Order)
				                      .ToImmutableArray();
			return result;
		}
	}
}