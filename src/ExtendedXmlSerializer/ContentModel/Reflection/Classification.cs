// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
//                    Michael DeMond
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Properties;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class Classification : IClassification
	{
		readonly IFormattedContentSpecification _specification;
		readonly IIdentityStore _identities;
		readonly IGenericTypes _generic;
		readonly ITypes _types;

		public Classification(IFormattedContentSpecification specification, IIdentityStore identities, IGenericTypes generic,
		                      ITypes types)
		{
			_specification = specification;
			_identities = identities;
			_generic = generic;
			_types = types;
		}

		public TypeInfo Get(IFormatReader parameter)
			=> FromAttributes(parameter) ?? _types.Get(_identities.Get(parameter.Name, parameter.Identifier));

		TypeInfo FromAttributes(IFormatReader parameter)
			=> _specification.IsSatisfiedBy(parameter)
				? ExplicitTypeProperty.Default.Get(parameter) ?? ItemTypeProperty.Default.Get(parameter) ?? Generic(parameter)
				: null;

		TypeInfo Generic(IFormatReader parameter)
		{
			var arguments = ArgumentsTypeProperty.Default.Get(parameter);
			var result = !arguments.IsDefault ? Generic(parameter, arguments) : null;
			return result;
		}

		TypeInfo Generic(IIdentity parameter, ImmutableArray<Type> arguments)
		{
			var candidates = _generic.Get(_identities.Get(parameter.Name, parameter.Identifier));
			var length = arguments.Length;
			foreach (var candidate in candidates)
			{
				if (candidate.GetGenericArguments().Length == length)
				{
					return candidate.MakeGenericType(arguments.ToArray())
					                .GetTypeInfo();
				}
			}
			return null;
		}
	}
}