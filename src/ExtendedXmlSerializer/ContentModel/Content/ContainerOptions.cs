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
using ExtendedXmlSerialization.ContentModel.Collections;
using ExtendedXmlSerialization.ContentModel.Converters;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.ContentModel.Content
{
	class ContainerOptions : IContainerOptions
	{
		readonly IContentOption _known;

		public ContainerOptions() : this(OptimizedConverterAlteration.Default) {}

		public ContainerOptions(IAlteration<IConverter> alteration) : this(new ContentOptions(alteration)) {}

		public ContainerOptions(IContentOption known)
		{
			_known = known;
		}

		public IEnumerable<IContainerOption> Get(ISerialization parameter)
		{
			var runtime = new RuntimeSerializer(parameter);
			var variable = new VariableTypeMemberOption(parameter, runtime);
			var members = new Members.Members(parameter, new Selector(variable));
			var options = ElementOptions.Default;

			yield return new ContainerOption(ElementOption.Default, _known);

			yield return new ContainerOption(ArrayElementOption.Default, new ArrayContentOption(parameter));
			yield return new ContainerOption(options,
			                                 new DictionaryContentOption(members, new DictionaryEntries(parameter, variable)));
			yield return new ContainerOption(options, new CollectionContentOption(members, parameter));

			yield return new ContainerOption(options, new MemberedContentOption(members));
			yield return new ContainerOption(options, new RuntimeContentOption(runtime));
		}
	}
}