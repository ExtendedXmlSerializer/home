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

using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	public sealed class ReaderContextExtension : ISerializerExtension
	{
		public static ReaderContextExtension Default { get; } = new ReaderContextExtension();
		ReaderContextExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate<IAlteration<IContentHandler>, Wrapper>();

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class Wrapper : IAlteration<IContentHandler>
		{
			readonly IAlteration<IContentHandler> _handler;

			public Wrapper(IAlteration<IContentHandler> handler)
			{
				_handler = handler;
			}

			public IContentHandler Get(IContentHandler parameter) => new Handler(_handler.Get(parameter));

			sealed class Handler : IContentHandler
			{
				readonly IContentsHistory _contexts;
				readonly IContentHandler _handler;

				public Handler(IContentHandler handler) : this(ContentsHistory.Default, handler) {}

				public Handler(IContentsHistory contexts, IContentHandler handler)
				{
					_contexts = contexts;
					_handler = handler;
				}

				public void Execute(IContents parameter)
				{
					var contexts = _contexts.Get(parameter.Get());
					contexts.Push(parameter);
					_handler.Execute(parameter);
					contexts.Pop();
				}
			}
		}
	}
}