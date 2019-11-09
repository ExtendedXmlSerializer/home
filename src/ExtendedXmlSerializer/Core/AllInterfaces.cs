using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.Core
{
	sealed class AllInterfaces : StructureCacheBase<TypeInfo, ImmutableArray<TypeInfo>>, IAllInterfaces
	{
		readonly Func<TypeInfo, IEnumerable<TypeInfo>> _selector;

		public static AllInterfaces Default { get; } = new AllInterfaces();

		AllInterfaces() => _selector = Yield;

		IEnumerable<TypeInfo> Yield(TypeInfo parameter) =>
			parameter.Yield()
			         .Concat(parameter.ImplementedInterfaces.YieldMetadata()
			                          .SelectMany(_selector))
			         .Where(x => x.IsInterface)
			         .Distinct();

		protected override ImmutableArray<TypeInfo> Create(TypeInfo parameter) => Yield(parameter)
			.ToImmutableArray();
	}
}