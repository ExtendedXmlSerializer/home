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

using System.Collections;
using System.Reflection;

namespace ExtendedXmlSerialization.ContentModel.Members
{
	class ReadOnlyMemberAdapter : IMemberAdapter
	{
		readonly IMemberAdapter _adapter;
		public ReadOnlyMemberAdapter(IMemberAdapter adapter)
		{
			_adapter = adapter;
		}

		public string Identifier => _adapter.Identifier;

		public string Name => _adapter.Name;

		public MemberInfo Metadata => _adapter.Metadata;

		public TypeInfo MemberType => _adapter.MemberType;

		public bool IsWritable => _adapter.IsWritable;

		public object Get(object instance) => _adapter.Get(instance);

		public void Assign(object instance, object value)
		{
			var collection = _adapter.Get(instance);
			foreach (var element in (IEnumerable) value)
			{
				_adapter.Assign(collection, element);
			}
		}
	}
}