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

using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	public sealed class DeferredReferencesExtension : ISerializerExtension
	{
		public static DeferredReferencesExtension Default { get; } = new DeferredReferencesExtension();
		DeferredReferencesExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate<IContentsResult, ContentsResult>()
			            .Decorate<IReferenceMaps, DeferredReferenceMaps>()
			            .Decorate<IContents, DeferredReferenceContents>()
			            .Decorate<ISerializers, DeferredReferenceSerializers>()
			            .Decorate<IReferenceEncounters, DeferredReferenceEncounters>();

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class ContentsResult : IContentsResult
		{
			readonly ICommand<IContentAdapter> _member;
			readonly ICommand<IContentAdapter> _collection;
			readonly IContentsResult _results;

			[UsedImplicitly]
			public ContentsResult(IContentsResult results)
				: this(
					ExecuteDeferredCommandsCommand<DeferredMemberAssignmentCommand>.Default,
					ExecuteDeferredCommandsCommand<DeferredCollectionAssignmentCommand>.Default, results) {}

			public ContentsResult(ICommand<IContentAdapter> member, ICommand<IContentAdapter> collection,
			                      IContentsResult results)
			{
				_member = member;
				_collection = collection;
				_results = results;
			}

			public object Get(IContentsAdapter parameter)
			{
				var adapter = parameter.Get();
				_member.Execute(adapter);
				_collection.Execute(adapter);

				var result = _results.Get(parameter);
				return result;
			}
		}
	}
}