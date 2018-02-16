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
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Content.Members;
using ExtendedXmlSerializer.ExtensionModel.Services;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Registration
{
	sealed class RegisteredContents<T> : ConditionalContents<T>, IContents<T>
	{
		public RegisteredContents(IRegisteredSerializers<T> serializers, IContents<T> fallback)
			: base(serializers, serializers, fallback) {}
	}

	sealed class RegisteredMemberContents<T> : ConditionalMemberContents<T>, IMemberContents<T>
	{
		public RegisteredMemberContents(IRegisteredSerializers<T> serializers, IMemberContents<T> contents)
			: base(serializers, contents) { }
	}


	interface IRegisteredSerializers : IMetadata<IContentSerializer<object>> {}

	sealed class RegisteredSerializers : MetadataTable<IContentSerializer<object>, IRegisteredSerializers>, IRegisteredSerializers { }

	public interface IRegisteredSerializers<T> : ISpecificationSource<MemberInfo, IContentSerializer<T>> { }

	sealed class RegisteredSerializers<T> : ServiceContainer<MemberInfo, IContentSerializer<T>>, IRegisteredSerializers<T>
	{
		public RegisteredSerializers(System.IServiceProvider provider, IRegisteredSerializers table)
			: base(provider, table.To(A<IService<IContentSerializer<T>>>.Default)) { }
	}

	sealed class ActivatedContentSerializer<T, TSerializer> : GenerializedContentSerializer<T>
		where TSerializer : IContentSerializer<T>
	{
		public ActivatedContentSerializer(TSerializer serializer) : base(serializer) {}
	}

	sealed class RegisteredSerializersExtension : ISerializerExtension, IAssignable<MemberInfo, IService<IContentSerializer<object>>>, ICommand<MemberInfo>
	{
		readonly IMetadataTable<IContentSerializer<object>> _serializers;

		[UsedImplicitly]
		public RegisteredSerializersExtension() : this(new RegisteredSerializers()) { }

		public RegisteredSerializersExtension(IMetadataTable<IContentSerializer<object>> serializers) => _serializers = serializers;

		public IServiceRepository Get(IServiceRepository parameter)
			=> _serializers.Get(parameter)
			               .RegisterDefinition<IRegisteredSerializers<object>, RegisteredSerializers<object>>()
			               .DecorateDefinition<IContents<object>, RegisteredContents<object>>()
			               .DecorateDefinition<IMemberContents<object>, RegisteredMemberContents<object>>();

		void ICommand<IServices>.Execute(IServices parameter) { }

		public void Execute(MemberInfo parameter)
		{
			_serializers.Remove(parameter);
		}

		public void Execute(KeyValuePair<MemberInfo, IService<IContentSerializer<object>>> parameter)
		{
			_serializers.Execute(parameter);
		}
	}
}