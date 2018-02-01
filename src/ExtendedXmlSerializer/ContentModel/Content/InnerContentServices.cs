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

using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class InnerContentServices : IInnerContentServices
	{
		readonly IListContentsSpecification _specification;
		readonly IInnerContentActivation _activation;
		readonly IAlteration<IInnerContentHandler> _handler;
		readonly IInnerContentResult _results;
		readonly IMemberHandler _member;
		readonly ICollectionContentsHandler _collection;
		readonly IReaderFormatter _formatter;

		public InnerContentServices(IListContentsSpecification specification, IInnerContentActivation activation,
		                        IAlteration<IInnerContentHandler> handler, IInnerContentResult results, IMemberHandler member,
		                        ICollectionContentsHandler collection, IReaderFormatter formatter)
		{
			_specification = specification;
			_activation = activation;
			_handler = handler;
			_results = results;
			_member = member;
			_collection = collection;
			_formatter = formatter;
		}

		public bool IsSatisfiedBy(IInnerContent parameter) => _specification.IsSatisfiedBy(parameter);

		public string Get(IFormatReader parameter) => _formatter.Get(parameter);

		public IReader Create(TypeInfo classification, IInnerContentHandler handler)
			=> new InnerContentReader(_activation.Get(classification), _handler.Get(handler), _results);

		public void Handle(IInnerContent contents, IMemberSerializer member) => _member.Handle(contents, member);
		public void Handle(IListInnerContent contents, IReader reader) => _collection.Handle(contents, reader);
	}
}