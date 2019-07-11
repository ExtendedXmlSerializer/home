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

using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	sealed class ReaderContextExtension : ISerializerExtension
	{
		public static ReaderContextExtension Default { get; } = new ReaderContextExtension();
		ReaderContextExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate<IAlteration<IInnerContentHandler>, Wrapper>();

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class Wrapper : IAlteration<IInnerContentHandler>
		{
			readonly IAlteration<IInnerContentHandler> _handler;

			public Wrapper(IAlteration<IInnerContentHandler> handler)
			{
				_handler = handler;
			}

			public IInnerContentHandler Get(IInnerContentHandler parameter) => new Handler(_handler.Get(parameter));

			sealed class Handler : IInnerContentHandler
			{
				readonly IContentsHistory _contexts;
				readonly IInnerContentHandler _handler;

				public Handler(IInnerContentHandler handler) : this(ContentsHistory.Default, handler) {}

				public Handler(IContentsHistory contexts, IInnerContentHandler handler)
				{
					_contexts = contexts;
					_handler = handler;
				}

				public bool IsSatisfiedBy(IInnerContent parameter)
				{
					var contexts = _contexts.Get(parameter.Get());
					contexts.Push(parameter);
					var result = _handler.IsSatisfiedBy(parameter);
					contexts.Pop();
					return result;
				}
			}
		}
	}
}