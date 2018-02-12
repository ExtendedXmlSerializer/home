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

using System.Collections.Generic;

namespace ExtendedXmlSerializer.Core.Sources
{
	public interface ITableAssignment<TKey, TValue> : IAssignable<TKey, TValue> {}

	sealed class TableAssignment<TKey, TValue> : ITableAssignment<TKey, TValue>
	{
		readonly ITableSource<TKey, TValue> _source;

		public TableAssignment(ITableSource<TKey, TValue> source) => _source = source;

		public void Execute(KeyValuePair<TKey, TValue> parameter)
		{
			Assign(parameter.Key, parameter.Value);
		}

		public void Assign(TKey key, TValue value)
		{
			if (value == null)
			{
				_source.Remove(key);
			}
			else
			{
				_source.Assign(key, value);
			}
		}
	}
}