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
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ContentModel.Properties;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.TypeModel;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	sealed class MarkupExtensions : ReferenceCacheBase<MarkupExtensionParts, IMarkupExtension>, IMarkupExtensions
	{
		readonly Func<string, object> _evaluator;
		readonly ITypeParser _parser;
		readonly ITypeMembers _members;
		readonly IMemberAccessors _accessors;
		readonly IConstructors _constructors;
		readonly Func<KeyValuePair<string, string>, object> _selector;

		public MarkupExtensions(Func<string, object> evaluator, ITypeParser parser, ITypeMembers members,
		                        IMemberAccessors accessors, IConstructors constructors)
		{
			_evaluator = evaluator;
			_parser = parser;
			_members = members;
			_accessors = accessors;
			_constructors = constructors;
			_selector = Create;
		}

		object Create(KeyValuePair<string, string> arg) => _evaluator.Invoke(arg.Value);

		protected override IMarkupExtension Create(MarkupExtensionParts parameter)
		{
			var values = parameter.Arguments.Select(_evaluator).ToArray();
			var specification = new ValidMarkupExtensionConstructor(values.ToImmutableArray());
			var constructors = new QueriedConstructors(specification, _constructors);
			var type = _parser.Get(parameter.Type);
			var constructor = constructors.Get(type);
			if (constructor == null)
			{
				throw new InvalidOperationException(
					$"An attempt was made to describe a markup extension with two parameters with the values '{string.Join(", ", values)}', but a constructor could not be found on the markup extension type that takes the following types in order: '{string.Join(", ", values.Select(x => x?.GetType()))}'.");
			}

			var dictionary = parameter.Properties.ToDictionary(x => x.Key, _selector);
			var activator = new ConstructedActivator(constructor, values);
			var result = new ActivationContexts(_accessors, _members.Get(type), activator)
				.Get(dictionary)
				.Get()
				.AsValid<IMarkupExtension>();
			return result;
		}
	}
}