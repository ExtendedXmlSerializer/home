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
using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Services;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;
using System.Reactive;

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

	[Extension(typeof(AssignableContentsExtension))]
	public class EmitUnassignedSpecificationProperty : Property<ISpecification<Unit>>
	{
		[UsedImplicitly]
		public static EmitUnassignedSpecificationProperty Default { get; } = new EmitUnassignedSpecificationProperty();
		EmitUnassignedSpecificationProperty() : base(NeverSpecification<object>.Default.To(A<Unit>.Default).Accept) {}
	}

	public class AssignableContentsExtension : ISerializerExtension
	{
		public static AssignableContentsExtension Default { get; } = new AssignableContentsExtension();
		AssignableContentsExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterDefinition<INullContentReader<object>, NullContentReader<object>>()
			            .RegisterDefinition<INullContentWriter<object>, NullContentWriter<object>>()
			            .Decorate<AssignableContents<object>>();

		public void Execute(IServices parameter) {}

		sealed class NullContentReader<T> : Singleton<ContentModel.NullContentReader<T>, IFormatReader, T>, INullContentReader<T>
		{
			public NullContentReader(ISingletonLocator locator) : base(locator) {}
		}

		sealed class NullContentWriter<T> : SingletonCommand<ContentModel.NullContentWriter<T>, ContentModel.Writing<T>>, INullContentWriter<T>
		{
			public NullContentWriter(ISingletonLocator locator) : base(locator) {}
		}

	}

	public sealed class AssignableStructureContentsExtension : ISerializerExtension
	{
		public static AssignableStructureContentsExtension Default { get; } = new AssignableStructureContentsExtension();
		AssignableStructureContentsExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.DecorateContent<NullableContents>(AssignableStructureSpecification.Default)
			            .Decorate<AssignableStructureContents<object>>();

		public void Execute(IServices parameter) {}
	}

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