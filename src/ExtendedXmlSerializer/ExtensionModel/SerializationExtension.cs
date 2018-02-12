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
using ExtendedXmlSerializer.ExtensionModel.References;
using ExtendedXmlSerializer.ExtensionModel.Services;

namespace ExtendedXmlSerializer.ExtensionModel
{
	public sealed class SerializationExtension : ISerializerExtension
	{
		public static SerializationExtension Default { get; } = new SerializationExtension();
		SerializationExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterDefinition<IRead<object>, Read<object>>()
			            .RegisterDefinition<IWrite<object>, Write<object>>()
			            .RegisterDefinition<ISerializer<object, object>, Serializer<object, object>>()
			            .Register<ISerializer, RuntimeSerializer>()
			            .Register<ISerializers, GeneralizedSerializers>()
			            .RegisterDefinition<IContentSerializer<object>, RuntimeSerializer<object>>()
			            .RegisterDefinition<RuntimeContentSerializerSource<object>>()
			            .RegisterDefinition<IRuntimeContentSerializers<object>, RuntimeContentSerializers<object>>()
			            .RegisterDefinition<GeneralizedSerializers<object>>()
			            .RegisterDefinition<ISerializers<object>, Serializers<object>>()
			            .RegisterDefinition<IReaders<object>, Readers<object>>()
			            .RegisterDefinition<IWriters<object>, Writers<object>>()
			            .RegisterDefinition<ContentModel.IContentReaders<object>, ContentModel.ContentReaders<object>>()
			            .RegisterDefinition<ContentModel.IContentWriters<object>, ContentModel.ContentWriters<object>>()
			            .Register<ContentModel.Content.ISerializers, Serializers>()
			            .Decorate<ContentModel.Content.ISerializers, ReferenceAwareSerializers>()
			            .Decorate<IContents, RecursionAwareContents>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}