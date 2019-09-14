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

using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	sealed class RegisteredCompositionExtension : ISerializerExtension, IAssignable<TypeInfo, ISerializerComposer>
	{
		readonly ITypedTable<ISerializerComposer> _composers;

		[UsedImplicitly]
		public RegisteredCompositionExtension() : this(new TypedTable<ISerializerComposer>()) {}

		public RegisteredCompositionExtension(ITypedTable<ISerializerComposer> composers) => _composers = composers;

		public IServiceRepository Get(IServiceRepository parameter) => parameter.RegisterInstance(_composers)
		                                                                        .Decorate<IContents, Contents>();

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class Contents : IContents
		{
			readonly IContents                        _contents;
			readonly ITypedTable<ISerializerComposer> _table;

			public Contents(IContents contents, ITypedTable<ISerializerComposer> table)
			{
				_contents = contents;
				_table    = table;
			}

			public ISerializer Get(TypeInfo parameter)
			{
				var content = _contents.Get(parameter);
				var result = _table.IsSatisfiedBy(parameter)
					             ? _table.Get(parameter)
					                     .Get(content)
					             : content;
				return result;
			}
		}

		public void Assign(TypeInfo key, ISerializerComposer value)
		{
			_composers.Assign(key, value);
		}
	}
}