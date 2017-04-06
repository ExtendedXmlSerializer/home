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

using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel
{
	sealed class ContentsServices : IContentsServices
	{
		readonly IListContentsSpecification _specification;
		readonly IContentsActivation _activation;
		readonly IAlteration<IContentHandler> _handler;
		readonly IContentsResult _results;
		readonly IMemberAssignment _member;
		readonly ICollectionAssignment _collection;
		readonly IContentAdapterFormatter _formatter;

		public ContentsServices(IListContentsSpecification specification, IContentsActivation activation,
		                        IAlteration<IContentHandler> handler, IContentsResult results, IMemberAssignment member,
		                        ICollectionAssignment collection, IContentAdapterFormatter formatter)
		{
			_specification = specification;
			_activation = activation;
			_handler = handler;
			_results = results;
			_member = member;
			_collection = collection;
			_formatter = formatter;
		}

		public bool IsSatisfiedBy(IContentsAdapter parameter) => _specification.IsSatisfiedBy(parameter);

		public void Assign(IReader source, IContentsAdapter parameter, IMemberAccess access)
			=> _member.Assign(source, parameter, access);

		public void Assign(IReader source, IListContentsAdapter parameter) => _collection.Assign(source, parameter);

		public string Get(IContentAdapter parameter) => _formatter.Get(parameter);

		public IReader Create(TypeInfo classification, IContentHandler handler)
			=> new ContentsReader(_activation.Get(classification), _handler.Get(handler), _results);
	}
}