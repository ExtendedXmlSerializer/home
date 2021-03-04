using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ContentModel.Properties;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class Classification : IClassification
	{
		readonly IFormattedContentSpecification _specification;
		readonly IIdentityStore                 _identities;
		readonly IGenericTypes                  _generic;
		readonly ITypes                         _types;

		// ReSharper disable once TooManyDependencies
		public Classification(IFormattedContentSpecification specification, IIdentityStore identities,
		                      IGenericTypes generic,
		                      ITypes types)
		{
			_specification = specification;
			_identities    = identities;
			_generic       = generic;
			_types         = types;
		}

		public TypeInfo Get(IFormatReader parameter)
			=> FromAttributes(parameter) ?? (parameter.Contains(MemberIdentity.Default)
				                                 ? null
				                                 : _types.Get(_identities.Get(parameter.Name, parameter.Identifier)));
		TypeInfo FromAttributes(IFormatReader parameter)
			=> _specification.IsSatisfiedBy(parameter)
				   ? ExplicitTypeProperty.Default.Get(parameter) ??
				     ItemTypeProperty.Default.Get(parameter) ?? Generic(parameter)
				   : null;

		TypeInfo Generic(IFormatReader parameter)
		{
			var arguments = ArgumentsTypeProperty.Default.Get(parameter);
			var result    = !arguments.IsDefault ? Generic(parameter, arguments) : null;
			return result;
		}

		TypeInfo Generic(IIdentity parameter, ImmutableArray<Type> arguments)
		{
			var candidates = _generic.Get(_identities.Get(parameter.Name, parameter.Identifier));
			var target     = arguments.Length;

			var length = candidates.Length;
			for (var i = 0; i < length; i++)
			{
				var candidate = candidates[i];
				if (candidate.GetGenericArguments()
				             .Length == target)
				{
					return candidate.MakeGenericType(arguments.ToArray())
					                .GetTypeInfo();
				}
			}

			return null;
		}
	}
}