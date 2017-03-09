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

using ExtendedXmlSerialization.Configuration;
using ExtendedXmlSerialization.ContentModel.Converters;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.ExtensionModel;
using ExtendedXmlSerialization.Test.Support;
using Xunit;

namespace ExtendedXmlSerialization.Test.ExtensionModel
{
	public class OptimizedConvertersExtensionTests
	{
		[Fact]
		public void Verify()
		{
			var container = new OptimizedConverterAlteration();
			var alteration = new Alteration(container);
			var sut = new OptimizedConvertersExtension(alteration, container);
			var support = new SerializationSupport(new ExtendedXmlConfiguration().Extended(sut).Create());

			const float number = 4.5678f;
			var actual = support.Assert(number, @"<?xml version=""1.0"" encoding=""utf-8""?><float xmlns=""https://github.com/wojtpl2/ExtendedXmlSerializer/system"">4.5678</float>");
			Assert.Equal(number, actual);

			var converter = alteration.Get(FloatConverter.Default);
			var format = converter.Format(number);
			for (var i = 0; i < 10; i++)
			{
				Assert.Same(format, converter.Format(number));
			}
			container.Clear();
			Assert.NotSame(format, converter.Format(number));
		}

		class Alteration : CacheBase<IConverter,IConverter>, IAlteration<IConverter>
		{
			readonly IAlteration<IConverter> _converter;
			public Alteration(IAlteration<IConverter> converter)
			{
				_converter = converter;
			}

			protected override IConverter Create(IConverter parameter) => _converter.Get(parameter);
		}
	}
}