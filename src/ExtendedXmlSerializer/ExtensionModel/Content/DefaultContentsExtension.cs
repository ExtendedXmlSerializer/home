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
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Services;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	public sealed class DefaultContentsExtension : ISerializerExtension
	{
		public static DefaultContentsExtension Default { get; } = new DefaultContentsExtension();
		DefaultContentsExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Register<IContents, RuntimeContents>()
			            .Register(provider => new DeferredContents(provider.Get<IContents>))
			            .RegisterDefinition<DeferredContents<object>>()
			            .RegisterDefinition<IContents<object>, ContentModel.Content.RuntimeContents<object>>()
			            .RegisterDefinition<AlteredContentSerializer<object, object>>()
			            .RegisterDefinition<IAlteredContents<object>, AlteredContents<object>>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}

	public sealed class NullableStructureContentsExtension : ISerializerExtension
	{
		public static NullableStructureContentsExtension Default { get; } = new NullableStructureContentsExtension();
		NullableStructureContentsExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.DecorateContent<NullableContents>(AssignableStructureSpecification.Default)
			            .Decorate<AssignableStructureContents<object>>();

		public void Execute(IServices parameter) {}
	}

	/*public sealed class MetadataContentsExtension : ISerializerExtension
	{
		public static MetadataContentsExtension Default { get; } = new MetadataContentsExtension();
		MetadataContentsExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterInstance(ReflectionSerializer.Default)
			            .DecorateContent<ReflectionContents>(ReflectionContentSpecification.Default)
			            .Decorate<ReflectionRegistration<object>>();

		public void Execute(IServices parameter) {}
	}*/

	public sealed class MemberedContentsExtension : ISerializerExtension
	{
		public static MemberedContentsExtension Default { get; } = new MemberedContentsExtension();
		MemberedContentsExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.DecorateContent<IActivatingTypeSpecification, MemberedContents>()
			            .Decorate<MemberedContentsRegistration<object>>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}

	public sealed class CollectionContentsExtension : ISerializerExtension
	{
		public static CollectionContentsExtension Default { get; } = new CollectionContentsExtension();
		CollectionContentsExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.DecorateContent<DefaultCollectionSpecification, DefaultCollections>()
			            .Decorate<CollectionsRegistration<object>>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}

	public sealed class DictionaryContentsExtension : ISerializerExtension
	{
		public static DictionaryContentsExtension Default { get; } = new DictionaryContentsExtension();
		DictionaryContentsExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Register<IDictionaryEntries, DictionaryEntries>()
			            .DecorateContent<DictionaryContentSpecification, DictionaryContents>()
			            .Decorate<DictionaryRegistration<object>>();

		public void Execute(IServices parameter) {}
	}

	public sealed class ArrayContentsExtension : ISerializerExtension
	{
		public static ArrayContentsExtension Default { get; } = new ArrayContentsExtension();
		ArrayContentsExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.DecorateContent<Arrays>(ArraySpecification.Default).Decorate<ArrayContentsRegistration<object>>();

		public void Execute(IServices parameter) {}
	}
}