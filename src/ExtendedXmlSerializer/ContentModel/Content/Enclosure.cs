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

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class Enclosure : Enclosure<object>
	{
		public Enclosure(IWriter<object> start, IWriter<object> body) : base(start, body) {}
	}

	class Enclosure<T> : IContentWriter<T>, IWriter<T>
	{
		readonly IWriter<T> _start;
		readonly IWriter<T> _body;
		readonly IWriter<T> _finish;

		public Enclosure(IWriter<T> start, IWriter<T> body) : this(start, body, EndCurrentElement<T>.Default) {}

		public Enclosure(IWriter<T> start, IWriter<T> body, IWriter<T> finish)
		{
			_start = start;
			_body = body;
			_finish = finish;
		}

		public void Write(IFormatWriter writer, T instance)
		{
			_start.Write(writer, instance);
			_body.Write(writer, instance);
			_finish.Write(writer, instance);
		}

		public void Execute(Writing<T> parameter)
		{
			_start.Write(parameter.Writer, parameter.Instance);
			_body.Write(parameter.Writer, parameter.Instance);
			_finish.Write(parameter.Writer, parameter.Instance);
		}
	}
}