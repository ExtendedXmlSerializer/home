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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Formatting;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ContentModel.Parsing;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Expressions;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.TypeModel;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	sealed class MarkupExtensions : ReferenceCacheBase<MarkupExtensionParts, IMarkupExtension>, IMarkupExtensions
	{
		const string Extension = "Extension";

		readonly IEvaluator _evaluator;
		readonly IReflectionParser _parser;
		readonly ITypeMembers _members;
		readonly IMemberAccessors _accessors;
		readonly IConstructors _constructors;

		public MarkupExtensions(IEvaluator evaluator, IReflectionParser parser, ITypeMembers members,
		                        IMemberAccessors accessors, IConstructors constructors)
		{
			_evaluator = evaluator;
			_parser = parser;
			_members = members;
			_accessors = accessors;
			_constructors = constructors;
		}

		protected override IMarkupExtension Create(MarkupExtensionParts parameter)
		{
			var type = DetermineType(parameter);

			var candidates = parameter.Arguments.Select(_evaluator.Get).ToArray();
			var constructor = DetermineConstructor(parameter, candidates, type);

			var members = _members.Get(type);
			var evaluator = new PropertyEvaluator(type, members.ToDictionary(x => x.Name), _evaluator);
			var dictionary = parameter.Properties.ToDictionary(x => x.Key, evaluator.Get);
			var arguments = constructor.GetParameters()
			                           .Select(x => x.ParameterType.GetTypeInfo())
			                           .Zip(candidates, (info, evaluation) => evaluation.Get(info))
			                           .ToArray();
			var activator = new ConstructedActivator(constructor, arguments);
			var result = new ActivationContexts(_accessors, members, activator).Get(dictionary)
			                                                                   .Get()
			                                                                   .AsValid<IMarkupExtension>();
			return result;
		}

		ConstructorInfo DetermineConstructor(MarkupExtensionParts parameter, IEvaluation[] candidates, TypeInfo type)
		{
			var specification = new ValidMarkupExtensionConstructor(candidates.ToImmutableArray());
			var constructors = new QueriedConstructors(specification, _constructors);
			var constructor = constructors.Get(type);
			if (constructor == null)
			{
				var values = parameter.Arguments.Select(x => x.Get()).ToArray();

				var primary = new InvalidOperationException(
					$"An attempt was made to activate a markup extension of type '{type}' and the constructor parameters values '{string.Join(", ", values)}', but a constructor could not be located that would accept these values. Please see any associated exceptions for any errors encountered while evaluating these parameter values.");

				throw new AggregateException(primary.Yield().Concat(candidates.Select(x => x.Get()).Where(x => x != null)));
			}
			return constructor;
		}

		TypeInfo DetermineType(MarkupExtensionParts parameter)
		{
			var type = _parser.Get(parameter.Type) ?? _parser.Get(Copy(parameter.Type));
			if (type == null)
			{
				var name = IdentityFormatter<TypeParts>.Default.Get(parameter.Type);
				throw new InvalidOperationException($"Could not find a markup extension with the provided data: {name}");
			}

			if (!IsAssignableSpecification<IMarkupExtension>.Default.IsSatisfiedBy(type))
			{
				throw new InvalidOperationException($"{type} does not implement IMarkupExtension.");
			}
			return type;
		}

		static TypeParts Copy(TypeParts parameter)
		{
			var array = parameter.GetArguments()?.ToArray();
			var result = new TypeParts(string.Concat(parameter.Name, Extension), parameter.Identifier,
			                           array != null ? array.ToImmutableArray : (Func<ImmutableArray<TypeParts>>) null);
			return result;
		}

		sealed class PropertyEvaluator : IParameterizedSource<KeyValuePair<string, IExpression>, object>
		{
			readonly TypeInfo _type;
			readonly IDictionary<string, IMember> _members;
			readonly IEvaluator _evaluator;

			public PropertyEvaluator(TypeInfo type, IDictionary<string, IMember> members, IEvaluator evaluator)
			{
				_type = type;
				_members = members;
				_evaluator = evaluator;
			}

			public object Get(KeyValuePair<string, IExpression> parameter)
			{
				var evaluation = _evaluator.Get(parameter.Value);
				var member = _members.Get(parameter.Key);

				if (member == null)
				{
					throw new InvalidOperationException(
						$"The member '{parameter.Key}' was used to define a property of type '{_type}', but this property does not exist, or is not serializable.");
				}

				if (!evaluation.IsSatisfiedBy(member.MemberType))
				{
					var primary = new InvalidOperationException(
						$"An attempt was made to assign the property '{parameter.Key}' of a markup extension of type '{_type}' with the value '{parameter.Value}', but this value could not be assigned to this property. Please see any associated exceptions for any errors encountered while evaluating these parameter values.");

					throw new AggregateException(primary, evaluation.Get());
				}

				return evaluation.Get(member.MemberType);
			}
		}
	}
}