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

using System.Collections.Generic;
using System.Linq;
using ExtendedXmlSerialization.ContentModel.Collections;
using ExtendedXmlSerialization.ContentModel.Converters;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.ContentModel.Content
{
	class ContainerDefinitions : IContainerDefinitions
	{
		readonly IContentOption _known;

		public ContainerDefinitions() : this(OptimizedConverterAlteration.Default) {}

		public ContainerDefinitions(IAlteration<IConverter> alteration)
			: this(new CompositeContentOption(new WellKnownContent(alteration).ToArray())) {}

		public ContainerDefinitions(IContentOption known)
		{
			_known = known;
		}

		public IEnumerable<ContainerDefinition> Get(ISerializers parameter)
		{
			var runtime = new RuntimeSerializer(parameter);
			var variable = new VariableTypeMemberOption(parameter, runtime);
			var members = new Members.Members(parameter, new Selector(parameter, variable));

			yield return new ContainerDefinition(ElementOption.Default, _known);
			yield return new ContainerDefinition(ArrayElementOption.Default, new ArrayContentOption(parameter));
			yield return new ContainerDefinition(new DictionaryContentOption(members, variable));
			yield return new ContainerDefinition(new CollectionContentOption(parameter));

			yield return new ContainerDefinition(new MemberedContentOption(members));
			yield return new ContainerDefinition(new RuntimeContentOption(runtime));
		}
	}
}