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
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Conversion;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.ElementModel.Members;

namespace ExtendedXmlSerialization.ElementModel
{
	class ElementsSource : ElementsSourceBase
	{
		readonly IElementNames _names;
		readonly ISpecification<TypeInfo> _specification;

		public static ElementsSource Default { get; } = new ElementsSource();

		ElementsSource()
			: this(
				ElementNames.Default, Defaults.Names.Select(x => x.Classification),
				DictionaryEntryElement.DictionaryEntryType
			) {}

		protected ElementsSource(IElementNames names, IEnumerable<TypeInfo> known, params TypeInfo[] except)
			: this(names, new Specification(known, except)) {}

		protected ElementsSource(IElementNames names, ISpecification<TypeInfo> specification)
		{
			_names = names;
			_specification = specification;
		}

		protected override IEnumerable<IOption<TypeInfo, IElement>> CreateOptions(IElements parameter)
		{
			var members = new ElementMembers(CreateMembers(parameter));
			yield return new ElementOption(_specification, _names);
			yield return new DictionaryElementOption(parameter, _names, members);
			yield return new ArrayElementOption(_names, parameter);
			yield return new CollectionElementOption(parameter, _names, members);
			yield return new ActivatedElementOption(_names, members);
			yield return new ElementOption(_names);
		}

		protected virtual IMemberElementSelector CreateMembers(IElements parameter) => new MemberElementSelector(parameter);

		sealed class Specification : AnySpecification<TypeInfo>
		{
			public Specification(IEnumerable<TypeInfo> known, params TypeInfo[] except)
				: base(
					new AnySpecification<TypeInfo>(new ContainsSpecification<TypeInfo>(known.Except(except).ToArray()),
					                               IsAssignableSpecification<Enum>.Default)) {}
		}
	}
}