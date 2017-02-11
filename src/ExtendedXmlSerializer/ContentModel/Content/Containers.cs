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
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.ContentModel.Content
{
	class Containers : IContainers
	{
		// public static Containers Default { get; } = new Containers();
		readonly static Func<IContainers, IEnumerable<ContainerDefinition>> Options = ContainerDefinitions.Default.Get;

		readonly IParameterizedSource<TypeInfo, ISerializer> _selector, _contents;

		public Containers() : this(Options) {}

		public Containers(Func<IContainers, IEnumerable<ContainerDefinition>> options)
		{
			var definitions = options(this).ToArray();
			var option = definitions.Select(x => new ContainerOption(x.Element, x.Content)).ToArray();
			_selector = new Selector<TypeInfo, ISerializer>(option).Cache();
			_contents = new Selector<TypeInfo, ISerializer>(definitions.Select(x => x.Content).ToArray()).Cache();
		}

		public ISerializer Get(TypeInfo parameter) => _selector.Get(parameter);
		public ISerializer Content(TypeInfo parameter) => _contents.Get(parameter);
	}
}