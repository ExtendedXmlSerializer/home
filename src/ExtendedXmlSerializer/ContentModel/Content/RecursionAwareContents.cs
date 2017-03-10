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

using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.ContentModel.Content
{
	sealed class RecursionAwareContents : CacheBase<TypeInfo, ISerializer>, IContents
	{
		readonly IContents _contents;
		readonly ISet<int> _types; // TODO: Originally made this an ISet<TypeInfo> but suffered from GC quirks in benchmarks.

		public RecursionAwareContents(IContents contents) : this(contents, new HashSet<int>()) {}

		public RecursionAwareContents(IContents contents, ISet<int> types)
		{
			_contents = contents;
			_types = types;
		}

		protected override ISerializer Create(TypeInfo parameter)
		{
			var code = parameter.GetHashCode();
			var result = _types.Add(code)
				? _contents.Get(parameter)
				: new DeferredSerializer(_contents.Build(parameter));
			_types.Remove(code);
			return result;
		}
	}
}