using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Sources;
using Sprache;
using System;
using System.Collections.Immutable;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Reflection;

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
		var type = (arguments.HasValue ? Generic(typeInfo, arguments.Value) : typeInfo)
		           ??
		           throw new
			           ParseException($"An attempt was made to parse the identity '{IdentityFormatter.Default.Get(identity)}', but no type could be located with that name.");
		var result = parameter.Dimensions.HasValue
			             ? new DimensionsAlteration(parameter.Dimensions.Value).Get(type)
			             : type;
		return result;
	}

	TypeInfo Generic(TypeInfo candidate, ImmutableArray<TypeParts> arguments)
	{
		var definition = candidate.Name.EndsWith($"`{arguments.Length}")
			                 ? candidate
			                 : Locate(candidate, arguments.Length);
		return definition.MakeGenericType(Arguments(arguments)).GetTypeInfo();
	}

	static Type Locate(TypeInfo candidate, int count)
	{
		var original = candidate.FullName;
		var name     = $"{original.Substring(0, original.IndexOf("`", StringComparison.Ordinal))}`{count}";
		return candidate.Assembly.GetType(name, true);
	}

	Type[] Arguments(ImmutableArray<TypeParts> names)
	{
		var length = names.Length;
		var result = new Type[length];
		for (var i = 0; i < length; i++)
		{
			result[i] = Get(names[i]);
		}

		return result;
	}
}