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

namespace ExtendedXmlSerializer.ContentModel.Collections
{
	sealed class ListContentHandler : IContentHandler
	{
		readonly IReader _item;
		readonly ICollectionAssignment _collection;

		public ListContentHandler(IReader item, ICollectionAssignment collection)
		{
			_item = item;
			_collection = collection;
		}

		public void Execute(IContentsAdapter parameter)
		{
			var list = (IListContentsAdapter) parameter;
			var content = list.Get();
			var item = _item.Get(content);
			_collection.Assign(content, list.List, list.Current, item);
		}
	}

	/*sealed class ListContentHandler : DecoratedSpecification<IContentsAdapter>, IContentHandler
	{
		public ListContentHandler(IListContentsSpecification specification, ICommand<IContentsAdapter> command)
			: base(new CommandSpecification<IContentsAdapter>(specification, command)) {}
	}*/
}