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
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Services;
using ExtendedXmlSerializer.ReflectionModel;
using System;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	sealed class EnumerationContentsExtension : ISerializerExtension
	{
		public static EnumerationContentsExtension Default { get; } = new EnumerationContentsExtension();
		EnumerationContentsExtension() {}

		public IServiceRepository Get(IServiceRepository parameter) => parameter.Decorate<EnumerationContentsRegistration<object>>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}

	sealed class EnumerationContentsRegistration<T> : ConditionalContents<T>, IContents<T>
	{
		public EnumerationContentsRegistration(IContents<T> fallback)
			: base(IsAssignableSpecification<Enum>.Default, EnumerationContents<T>.Default, fallback) {}
	}

	sealed class EnumerationContents<T> : FixedInstanceSource<IContentSerializer<T>>
	{
		public static EnumerationContents<T> Default { get; } = new EnumerationContents<T>();

		EnumerationContents() :
			base(ContentModel.Serializers.New(EnumerationParser<T>.Default.If(AssignedSpecification<string>.Default),
			                                  StringCoercer<T>.Default)) {}
	}
}