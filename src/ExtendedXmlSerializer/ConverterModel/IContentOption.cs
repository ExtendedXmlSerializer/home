// MIT License
// 
// Copyright (c) 2016 Wojciech Nag�rski
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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.ConverterModel
{
	public interface IContainerOption : IOption<TypeInfo, IConverter> {}

	public interface IContainerOptions : IEnumerable<IContainerOption> {}

	class ContainerOptions : IContainerOptions
	{
		readonly IContainerOption _known;
		readonly IContentOptions _options;

		public ContainerOptions(IContainers containers)
			: this(ContainerOption.Default, new ContentOptions(containers, new Contents(containers))) {}

		public ContainerOptions(IContainerOption known, IContentOptions options)
		{
			_known = known;
			_options = options;
		}

		public IEnumerator<IContainerOption> GetEnumerator()
		{
			yield return _known;
			foreach (var content in _options.Skip(1))
			{
				yield return new ContainerOption(Elements.Default, content);
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	public interface IContentOption : IOption<TypeInfo, IConverter> {}

	class ContentOption<T> : FixedOption<TypeInfo, IConverter>, IContentOption
	{
		public ContentOption(IConverter<T> converter) : this(converter, TypeEqualitySpecification<T>.Default) {}
		public ContentOption(IConverter<T> converter, ISpecification<TypeInfo> specification) : base(specification, converter) {}
	}

	class ContainerOption : OptionBase<TypeInfo, IConverter>, IContainerOption
	{
		public static ContainerOption Default { get; } = new ContainerOption();
		ContainerOption() : this(WellKnownContent.Default) {}


		readonly IParameterizedSource<TypeInfo, IWriter> _element;
		readonly IContentOption _content;

		public ContainerOption(IContentOption content) : this(ElementOption.Default, content) {}

		public ContainerOption(IParameterizedSource<TypeInfo, IWriter> element, IContentOption content) : base(content)
		{
			_element = element;
			_content = content;
		}

		public override IConverter Get(TypeInfo parameter) => new Container(_element.Get(parameter), _content.Get(parameter));
	}
}