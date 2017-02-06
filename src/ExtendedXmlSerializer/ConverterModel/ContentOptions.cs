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
using System.Collections.Generic;
using ExtendedXmlSerialization.ConverterModel.Collections;
using ExtendedXmlSerialization.ConverterModel.Members;

namespace ExtendedXmlSerialization.ConverterModel
{
	class ContentOptions : IContentOptions
	{
		readonly IContainers _containers;
		readonly IContents _contents;
		readonly IContentOption _known;

		public ContentOptions(IContainers containers, IContents contents)
			: this(containers, contents, WellKnownContent.Default) {}

		public ContentOptions(IContainers containers, IContents contents, IContentOption known)
		{
			_containers = containers;
			_contents = contents;
			_known = known;
		}

		public IEnumerator<IContentOption> GetEnumerator()
		{
			yield return _known;

			yield return new ArrayOption(_containers);
			yield return new CollectionOption(_containers);

			yield return new MemberedContentOption(new Members.Members(_contents));
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}