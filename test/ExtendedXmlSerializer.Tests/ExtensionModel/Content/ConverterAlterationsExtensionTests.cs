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

using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Converters;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Tests.Support;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ExtensionModel.Content
{
	public class ConverterAlterationsExtensionTests
	{
		[Fact]
		public void Verify()
		{
			var container = new Optimizations();
			var sut = new Alteration(container);
			var support = new SerializationSupport(new ExtendedConfiguration().Alter(sut).Create());

			const float number = 4.5678f;
			var actual = support.Assert(number, @"<?xml version=""1.0"" encoding=""utf-8""?><float xmlns=""https://github.com/wojtpl2/ExtendedXmlSerializer/system"">4.5678</float>");
			Assert.Equal(number, actual);

			var converter = sut.Get(FloatConverter.Default);
			var format = converter.Format(number);
			for (var i = 0; i < 10; i++)
			{
				Assert.Same(format, converter.Format(number));
			}
			container.Clear();
			Assert.NotSame(format, converter.Format(number));
		}

		sealed class Alteration : CacheBase<IConverter,IConverter>, IAlteration<IConverter>
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