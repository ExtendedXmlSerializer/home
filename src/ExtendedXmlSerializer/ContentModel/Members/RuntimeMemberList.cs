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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ExtendedXmlSerialization.ContentModel.Members
{
	class RuntimeMemberList : IRuntimeMemberList
	{
		readonly Func<IMemberSerializer, bool> _specification;
		readonly IMemberSerializer[] _properties, _contents;
		readonly IRuntimeSerializer[] _runtime;

		public RuntimeMemberList(Func<IMemberSerializer, bool> specification, IEnumerable<IMemberSerializer> properties,
		                         IEnumerable<IRuntimeSerializer> runtime, IEnumerable<IMemberSerializer> contents)
		{
			_specification = specification;
			_properties = properties.ToArray();
			_runtime = runtime.ToArray();
			_contents = contents.ToArray();
		}

		public ImmutableArray<IMemberSerializer> Get(object parameter)
		{
			var runtime = Runtime(parameter).ToArray();
			var runtimeProperties = runtime.Where(_specification).ToArray();

			var properties = _properties.Concat(runtimeProperties).OrderBy(x => x.Profile.Order);
			var contents = _contents.Concat(runtime.Except(runtimeProperties)).OrderBy(x => x.Profile.Order);
			var result = properties.Concat(contents).ToImmutableArray();
			return result;
		}

		IEnumerable<IMemberSerializer> Runtime(object parameter)
		{
			var length = _runtime.Length;
			var result = new IMemberSerializer[length];
			for (var i = 0; i < length; i++)
			{
				result[i] = _runtime[i].Get(parameter);
			}
			return result;
		}
	}
}