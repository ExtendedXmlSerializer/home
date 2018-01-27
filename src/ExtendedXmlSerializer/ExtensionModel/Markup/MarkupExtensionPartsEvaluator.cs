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

using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ContentModel.Properties;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Sprache;
using ExtendedXmlSerializer.ExtensionModel.Expressions;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.Core.Parsing;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	sealed class MarkupExtensionPartsEvaluator
		: ReferenceCacheBase<MarkupExtensionParts, object>, IEvaluator,
		  IMarkupExtensionPartsEvaluator
	{
		const string Extension = "Extension";

		readonly IParser<MemberInfo> _parser;
		readonly IEvaluator _evaluator;
		readonly ITypeMembers _members;
		readonly IMemberAccessors _accessors;
		readonly IConstructors _constructors;
		readonly System.IServiceProvider _provider;
		readonly IFormatter<TypeParts> _formatter;
		readonly object[] _services;

		public MarkupExtensionPartsEvaluator(IParser<MemberInfo> parser, IEvaluator evaluator, ITypeMembers members,
		                                     IMemberAccessors accessors, IConstructors constructors,
		                                     System.IServiceProvider provider, params object[] services)
			: this(parser, evaluator, members, accessors, constructors, provider, TypePartsFormatter.Default, services) {}

		public MarkupExtensionPartsEvaluator(IParser<MemberInfo> parser, IEvaluator evaluator, ITypeMembers members,
		                                     IMemberAccessors accessors, IConstructors constructors,
		                                     System.IServiceProvider provider, IFormatter<TypeParts> formatter,
		                                     params object[] services)
		{
			_parser = parser;
			_evaluator = evaluator;
			_members = members;
			_accessors = accessors;
			_constructors = constructors;
			_provider = provider;
			_formatter = formatter;
			_services = services;
		}

		public IEvaluation Get(IExpression parameter)
		{
			var expression = parameter as MarkupExtensionPartsExpression;
			var accounted = expression != null ? new FixedExpression<object>(Get(expression.Get())) : parameter;
			var result = _evaluator.Get(accounted);
			return result;
		}

		protected override object Create(MarkupExtensionParts parameter)
		{
			var type = DetermineType(parameter);

			var candidates = parameter.Arguments.Select(Get)
			                          .ToArray();
			var constructor = DetermineConstructor(parameter, candidates, type);

			var members = _members.Get(type);
			var evaluator = new PropertyEvaluator(type, members.ToDictionary(x => x.Name), this);
			var dictionary = parameter.Properties.ToDictionary(x => x.Key, evaluator.Get);
			var arguments = constructor.GetParameters()
			                           .Select(x => x.ParameterType.GetTypeInfo())
			                           .Zip(candidates, (info, evaluation) => evaluation.Get(info))
			                           .ToArray();
			var activator = new ConstructedActivator(constructor, arguments);
			var extension = new ActivationContexts(_accessors, members, activator).Get(dictionary)
			                                                                            .Get()
			                                                                            .AsValid<IMarkupExtension>();
			var result = extension.ProvideValue(new Provider(_provider, _services.Appending(parameter)
			                                                                     .ToArray()));
			return result;
		}

		ConstructorInfo DetermineConstructor(MarkupExtensionParts parameter, IEvaluation[] candidates, TypeInfo type)
		{
			var specification = new ValidMarkupExtensionConstructor(candidates.ToImmutableArray());
			var constructors = new QueriedConstructors(specification, _constructors);
			var constructor = constructors.Get(type);
			if (constructor == null)
			{
				var values = parameter.Arguments.Select(x => x.ToString())
				                      .ToArray();

				var primary = new InvalidOperationException(
				                                            $"An attempt was made to activate a markup extension of type '{type}' and the constructor parameters values '{string.Join(", ", values)}', but a constructor could not be located that would accept these values. Please see any associated exceptions for any errors encountered while evaluating these parameter values.");

				var exceptions = primary.Yield()
				                        .Concat(candidates.Select(x => x.Get())
				                                          .Where(x => x != null));
				throw new AggregateException(exceptions);
			}
			return constructor;
		}

		TypeInfo DetermineType(MarkupExtensionParts parameter)
		{
			var type = TryParse(parameter.Type);
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

		TypeInfo TryParse(TypeParts parameter)
		{
			try
			{
				return Parse(parameter);
			}
			catch (ParseException)
			{
				return Parse(Copy(parameter));
			}
		}

		TypeInfo Parse(TypeParts parameter)
		{
			var s = _formatter.Get(parameter);
			return (TypeInfo) _parser.Get(s);
		}

		static TypeParts Copy(TypeParts parameter)
		{
			var array = parameter.GetArguments()
			                     ?.ToArray();
			var result = new TypeParts(string.Concat(parameter.Name, Extension), parameter.Identifier,
			                           array != null ? array.ToImmutableArray : (Func<ImmutableArray<TypeParts>>) null);
			return result;
		}

		sealed class PropertyEvaluator : IParameterizedSource<KeyValuePair<string, IExpression>, object>
		{
			readonly TypeInfo _type;
			readonly IReadOnlyDictionary<string, IMember> _members;
			readonly IEvaluator _evaluator;

			public PropertyEvaluator(TypeInfo type, IReadOnlyDictionary<string, IMember> members, IEvaluator evaluator)
			{
				_type = type;
				_members = members;
				_evaluator = evaluator;
			}

			public object Get(KeyValuePair<string, IExpression> parameter)
			{
				var member = _members.Get(parameter.Key);

				if (member == null)
				{
					throw new InvalidOperationException(
					                                    $"The member '{parameter.Key}' was used to define a property of type '{_type}', but this property does not exist, or is not serializable.");
				}

				var evaluation = _evaluator.Get(parameter.Value);
				if (!evaluation.IsSatisfiedBy(member.MemberType))
				{
					var innerException = evaluation.Get();
					if (innerException != null)
					{
						var primary =
							new InvalidOperationException(
							                              $"An attempt was made to assign the property '{parameter.Key}' of a markup extension of type '{_type}' with the expression value of '{parameter.Value}'.However, the resulting value of this expression could not be assigned to this property, which has the target type of '{member.MemberType}'. Please see any associated exceptions for any errors encountered while evaluating these parameter values.");
						throw new AggregateException(primary, innerException);
					}
					throw new InvalidOperationException(
					                                    $"An attempt was made to assign the property '{parameter.Key}' of a markup extension of type '{_type}' with the expression value of '{parameter.Value}'.  However, the resulting value of this expression could not be assigned to this property, which has the target type of '{member.MemberType}'.");
				}

				return evaluation.Get(member.MemberType);
			}
		}

		sealed class Provider : System.IServiceProvider
		{
			readonly System.IServiceProvider _provider, _services;

			public Provider(System.IServiceProvider provider, params object[] services)
				: this(provider, new ServiceProvider(services)) {}

			public Provider(System.IServiceProvider provider, System.IServiceProvider services)
			{
				_provider = provider;
				_services = services;
			}

			public object GetService(Type serviceType) => _services.GetService(serviceType) ?? _provider.GetService(serviceType);
		}
	}
}