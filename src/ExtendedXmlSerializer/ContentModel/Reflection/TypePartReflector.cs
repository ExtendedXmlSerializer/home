using System;
using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Sprache;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class TypePartReflector : CacheBase<TypeParts, TypeInfo>, ITypePartReflector
	{
		readonly IIdentityStore _identities;
		readonly ITypes         _types;

		public TypePartReflector(IIdentityStore identities, ITypes types) : base(TypePartsEqualityComparer.Default)
		{
			_identities = identities;
			_types      = types;
		}

		protected override TypeInfo Create(TypeParts parameter)
		{
			var identity  = _identities.Get(parameter.Name, parameter.Identifier);
			var typeInfo  = _types.Get(identity);
			var arguments = parameter.GetArguments();
			var type = arguments.HasValue
				           ? typeInfo.MakeGenericType(Arguments(arguments.Value))
				                     .GetTypeInfo()
				           : typeInfo;
			if (type == null)
			{
				throw new ParseException(
				                         $"An attempt was made to parse the identity '{IdentityFormatter.Default.Get(identity)}', but no type could be located with that name.");
			}

			var result = parameter.Dimensions.HasValue
				             ? new DimensionsAlteration(parameter.Dimensions.Value).Get(type)
				             : type;
			return result;
		}

		Type[] Arguments(ImmutableArray<TypeParts> names)
		{
			var length = names.Length;
			var result = new Type[length];
			for (var i = 0; i < length; i++)
			{
				result[i] = Get(names[i])
					.AsType();
			}

			return result;
		}
	}
}