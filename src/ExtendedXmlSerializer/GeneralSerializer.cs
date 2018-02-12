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
using System.IO;

namespace ExtendedXmlSerializer
{
	class GeneralizedSerializer<T> : GeneralSerializer<T>, ISerializer<T>
	{
		readonly ISerializer<T> _serializer;

		public GeneralizedSerializer(ISerializer<T> serializer) : base(serializer) => _serializer = serializer;

		public void Execute(Input<T> parameter) => _serializer.Execute(parameter);

		public T Get(Stream parameter) => _serializer.Get(parameter);
	}

	class GeneralSerializer<T> : ISerializer<object>
	{
		readonly ISerializer<T> _serializer;

		public GeneralSerializer(ISerializer<T> serializer) => _serializer = serializer;

		public void Execute(Input<object> parameter)
		{
			_serializer.Execute(new Input<T>(parameter.Stream, (T)parameter.Instance));
		}

		object IParameterizedSource<Stream, object>.Get(Stream parameter) => _serializer.Get(parameter);
	}
}