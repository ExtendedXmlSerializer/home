using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class TypePartResolver : StructureCacheBase<TypeInfo, TypeParts>, ITypePartResolver
	{
		readonly IIdentities                         _identities;
		readonly Func<TypeInfo, TypeInfo>            _root;
		readonly Func<TypeInfo, ImmutableArray<int>> _dimensions;

		public TypePartResolver(IIdentities identities) : this(identities, RootType.Default.Get,
		                                                       ArrayTypeDimensions.Default.Get) {}

		public TypePartResolver(IIdentities identities, Func<TypeInfo, TypeInfo> root,
		                        Func<TypeInfo, ImmutableArray<int>> dimensions)
		{
			_identities = identities;
			_root       = root;
			_dimensions = dimensions;
		}

		protected override TypeParts Create(TypeInfo parameter)
		{
			var array = parameter.IsArray;

			var identity   = _identities.Get(array ? _root(parameter) : parameter);
			var arguments  = parameter.IsGenericType ? Arguments(parameter.GetGenericArguments()) : null;
			var dimensions = array ? _dimensions(parameter) : (ImmutableArray<int>?)null;
			var result     = new TypeParts(identity.Name, identity.Identifier, arguments, dimensions);
			return result;
		}

		Func<ImmutableArray<TypeParts>> Arguments(IReadOnlyList<Type> types)
		{
			var length = types.Count;
			var names  = new TypeParts[length];
			for (var i = 0; i < length; i++)
			{
				names[i] = Get(types[i]
					               .GetTypeInfo());
			}

			var result = new Func<ImmutableArray<TypeParts>>(names.ToImmutableArray);
			return result;
		}
	}
}