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
using ExtendedXmlSerializer.Core;
using System.Collections.Generic;
using System.Linq;
using ISerializers = ExtendedXmlSerializer.ContentModel.Conversion.ISerializers;
using Serializers = ExtendedXmlSerializer.ContentModel.Conversion.Serializers;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	public sealed class ConvertersExtension : ISerializerExtension
	{
		readonly static IEnumerable<EnumerationConverters> Sources = EnumerationConverters.Default.Yield();

		readonly IEnumerable<IConverterSource> _sources;

		public ConvertersExtension() : this(WellKnownConverters.Default.KeyedByType(), Sources) {}

		public ConvertersExtension(ICollection<IConverter> converters, IEnumerable<IConverterSource> sources)
		{
			Converters = converters;
			_sources = sources;
		}

		public ICollection<IConverter> Converters { get; }

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterInstance(Converters.ToArray().Hide())
			            .RegisterInstance(_sources)
			            .Register<IConverters, Converters>()
			            .Register<ISerializers, Serializers>()
			            .Register<ConverterContentOption>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}