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
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.ConverterModel
{
	class Containers : IContainers
	{
		public static Containers Default { get; } = new Containers();
		Containers() : this(x => new ContainerDefinitions(x)) {}

		readonly IParameterizedSource<TypeInfo, IConverter> _selector, _contents;

		public Containers(Func<IContainers, IContainerDefinitions> options)
		{
			var definitions = options(this).ToArray();
			var option = definitions.Select(x => new ContainerOption(x.Element, x.Content)).ToArray();
			_selector = new Selector<TypeInfo, IConverter>(option).Cache();
			_contents = new Selector<TypeInfo, IConverter>(definitions.Select(x => x.Content).ToArray()).Cache();
		}

		public IConverter Get(TypeInfo parameter) => _selector.Get(parameter);
		public IConverter Content(TypeInfo parameter) => _contents.Get(parameter);
	}
}