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
using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Contents;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	sealed class ContentModelExtension : ISerializerExtension
	{
		public static ContentModelExtension Default { get; } = new ContentModelExtension();
		ContentModelExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Register<IComparer<IContentOption>, SortComparer<IContentOption>>()
			            .RegisterAsSet<IContentOption, ContentOptions>()
			            .RegisterAsStart<IContentOption, Start>()
			            .RegisterAsFinish<IContentOption, Finish>()
			            .Register<IDictionaryEntries, DictionaryEntries>()
			            .Register<ArrayContentOption>()
			            .Register<DictionaryContentOption>()
			            .Register<CollectionContentOption>()
			            .Register<IClassification, Classification>()
			            .Register<IIdentityStore, IdentityStore>()
			            .Register<IContentsServices, ContentsServices>()
			            .Register<IMemberHandler, MemberHandler>()
			            .Register<ICollectionContentsHandler, CollectionContentsHandler>()
			            .RegisterInstance<IAlteration<IContentHandler>>(Self<IContentHandler>.Default)
			            .RegisterInstance<IContentsResult>(ContentsResult.Default)
			            .RegisterInstance<IMemberAssignment>(MemberAssignment.Default)
			            .RegisterInstance<ICollectionAssignment>(CollectionAssignment.Default)
			            .RegisterInstance<IListContentsSpecification>(ListContentsSpecification.Default)
			            .RegisterInstance(ReflectionSerializer.Default);

		public void Execute(IServices parameter) => parameter.Get<ISortOrder>()
		                                                     .Sort<ArrayContentOption>(0)
		                                                     .Sort<DictionaryContentOption>(1)
		                                                     .Sort<CollectionContentOption>(2);
	}
}