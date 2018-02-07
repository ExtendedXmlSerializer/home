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

using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.References;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	public sealed class Contents : ISerializerExtension
	{
		public static Contents Default { get; } = new Contents();
		Contents() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterConstructorDependency<IContents>((provider, info) => provider.Get<DeferredContents>())
			            .Register<IDictionaryEntries, DictionaryEntries>()
			            .Register<IContents, RuntimeContents>()
			            .DecorateContent<IActivatingTypeSpecification, MemberedContents>()
			            .DecorateContent<DefaultCollectionSpecification, DefaultCollections>()
			            .DecorateContent<DictionaryContentSpecification, DictionaryContents>()
			            .DecorateContent<Arrays>(ArraySpecification.Default)
			            .RegisterInstance(ReflectionSerializer.Default)
			            .DecorateContent<ReflectionContents>(ReflectionContentSpecification.Default)
			            .DecorateContent<NullableContents>(IsNullableTypeSpecification.Default)
			            .DecorateContent<ConverterSpecification, ConverterContents>()
			            .DecorateContent<RegisteredContentSpecification, RegisteredContents>()
			            .RegisterDefinition<IContents<object>, RuntimeContents<object>>()
			            .RegisterDefinition<AlteredContentSerializers<object, object>>()
			            .RegisterDefinition<IAlteredContents<object>, AlteredContents<object>>()
			            .Decorate<MemberedContentsRegistration<object>>()
			            .Decorate<CollectionsRegistration<object>>()
			            .Decorate<DictionaryRegistration<object>>()
			            .Decorate<ArrayContentsRegistration<object>>()
			            .Decorate<ReflectionRegistration<object>>()
			            .Decorate<NullableRegistration<object>>()
			            .Decorate<ConverterContentsRegistration<object>>()
			            .Decorate<RegisteredRegistration<object>>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}