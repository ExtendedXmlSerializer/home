// MIT License
// 
// Copyright (c) 2016-2018 Wojciech Nagórski
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

using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	class AlteredContentSerializers<TFrom, TTo> : ISource<IContentSerializer<TFrom>>
	{
		readonly IContents<TTo> _contents;
		readonly IParameterizedSource<TTo, TFrom> _from;
		readonly IParameterizedSource<TFrom, TTo> _to;

		public AlteredContentSerializers(IContents<TTo> contents) : this(contents, CastCoercer<TTo, TFrom>.Default,
		                                                                 CastCoercer<TFrom, TTo>.Default) {}

		public AlteredContentSerializers(IContents<TTo> contents, IParameterizedSource<TTo, TFrom> from,
		                                 IParameterizedSource<TFrom, TTo> to)
		{
			_contents = contents;
			_from = from;
			_to = to;
		}

		public IContentSerializer<TFrom> Get() => new AlteredContentSerializer<TFrom, TTo>(_contents.Get(), _from, _to);
	}
}