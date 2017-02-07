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

using System.Collections;
using System.Collections.Generic;
using ExtendedXmlSerialization.ConverterModel.Collections;
using ExtendedXmlSerialization.ConverterModel.Members;

namespace ExtendedXmlSerialization.ConverterModel.Elements
{
	class ContainerDefinitions : IContainerDefinitions
	{
		readonly IContainers _containers;

		public ContainerDefinitions(IContainers containers)
		{
			_containers = containers;
		}

		public IEnumerator<ContainerDefinition> GetEnumerator()
		{
			yield return new ContainerDefinition(ElementOption.Default, WellKnownContent.Default);
			yield return new ContainerDefinition(ArrayElementOption.Default, new ArrayContentOption(_containers));
			yield return new ContainerDefinition(new DictionaryContentOption(_containers));
			yield return new ContainerDefinition(new CollectionContentOption(_containers));
			yield return new ContainerDefinition(new MemberedContentOption(new Members.Members(_containers)));
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}