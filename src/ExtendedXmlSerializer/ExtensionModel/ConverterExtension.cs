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
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.ContentModel.Converters;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.ExtensionModel
{
	class ConverterExtension : ISerializerExtension
	{
		readonly static IEnumerable<EnumerationConverters> Sources = EnumerationConverters.Default.Yield();

		public static ConverterExtension Default { get; } = new ConverterExtension();
		ConverterExtension() : this(WellKnownConverters.Default) {}

		readonly IEnumerable<IConverterSource> _sources;

		public ConverterExtension(IEnumerable<IConverter> converters) : this(converters.ToList(), Sources) {}

		public ConverterExtension(ICollection<IConverter> converters, IEnumerable<IConverterSource> sources)
		{
			Converters = converters;
			_sources = sources;
		}

		public ICollection<IConverter> Converters { get; }

		public IServiceRepository Get(IServiceRepository parameter)
		{
			return parameter
				.RegisterInstance<IEnumerable<IConverter>>(Converters)
				.RegisterInstance(_sources)
				.Register<IConverters, Converters>()
				.Register<ConverterContentOptions>()
				.Decorate<IEnumerable<IContentOption>>(
					(provider, options) => provider.Get<ConverterContentOptions>().Concat(options));
		}

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}