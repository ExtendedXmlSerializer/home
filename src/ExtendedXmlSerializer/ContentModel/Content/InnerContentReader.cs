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

using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	interface IContentsActivator<out T> : IContentReader<IInnerContent<T>> { }

	sealed class ContentsActivator<T> : IContentsActivator<T>
	{
		readonly IContentActivator<T> _activator;

		public ContentsActivator(IContentActivator<T> activator) => _activator = activator;

		public IInnerContent<T> Get(IFormatReader parameter)
			=> IdentityComparer.Default.Equals(parameter, NullElementIdentity.Default) ||
			   parameter.IsSatisfiedBy(NullValueIdentity.Default)
				   ? null
				   : _activator.Get(parameter);
	}

	interface IInnerContentReader<out T> : IContentReader<T> {}

	sealed class InnerContentReader<T> : IInnerContentReader<T>
	{
		readonly IContentsActivator<T> _activator;
		readonly IContentHandler<T> _handler;
		readonly IContentInstance<T> _instance;

		public InnerContentReader(IContentsActivator<T> activator, IContentHandler<T> handler, IContentInstance<T> instance)
		{
			_activator = activator;
			_handler = handler;
			_instance = instance;
		}

		public T Get(IFormatReader parameter)
		{
			var content = _activator.Get(parameter);
			if (content != null)
			{
				var input = new InnerContent<T>(parameter, content);
				while (content.MoveNext())
				{
					_handler.Execute(input);
				}

				return _instance.Get(content);
			}

			return default(T);
		}
	}


	sealed class InnerContentReader : IReader
	{
		readonly IInnerContents _activator;
		readonly IInnerContentHandler _content;
		readonly IInnerContentResult _result;

		public InnerContentReader(IInnerContents activator, IInnerContentHandler content, IInnerContentResult result)
		{
			_activator = activator;
			_content = content;
			_result = result;
		}

		public object Get(IFormatReader parameter)
		{
			var adapter = _activator.Get(parameter);
			while (adapter?.MoveNext() ?? false)
			{
				_content.Execute(adapter);
			}
			var result = adapter != null ? _result.Get(adapter) : null;
			return result;
		}
	}
}