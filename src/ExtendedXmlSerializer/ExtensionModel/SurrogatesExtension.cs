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
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace ExtendedXmlSerializer.ExtensionModel
{
	public sealed class SurrogatesExtension : ISerializerExtension
	{
		readonly ISerializationSurrogateProvider _provider;

		public SurrogatesExtension(ISerializationSurrogateProvider provider) => _provider = provider;

		public IServiceRepository Get(IServiceRepository parameter) => parameter.RegisterInstance(_provider)
		                                                                        .Decorate<IContents, Contents>();

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class Contents : IContents
		{
			readonly ISerializationSurrogateProvider _provider;
			readonly IContents _contents;

			public Contents(ISerializationSurrogateProvider provider, IContents contents)
			{
				_provider = provider;
				_contents = contents;
			}

			public ISerializer Get(TypeInfo parameter)
			{
				var surrogateType = _provider.GetSurrogateType(parameter);
				var result = surrogateType != null
					             ? new Serializer(_provider, new Mapping(parameter.AsType(), surrogateType),
					                              _contents.Get(surrogateType))
					             : _contents.Get(parameter);
				return result;
			}

			struct Mapping
			{
				public Mapping(Type origin, Type surrogate)
				{
					Origin = origin;
					Surrogate = surrogate;
				}

				public Type Origin { get; }

				public Type Surrogate { get; }
			}

			sealed class Serializer : ISerializer
			{
				readonly ISerializationSurrogateProvider _provider;
				readonly Mapping _mapping;
				readonly ISerializer _serializer;

				public Serializer(ISerializationSurrogateProvider provider, Mapping mapping, ISerializer serializer)
				{
					_provider = provider;
					_mapping = mapping;
					_serializer = serializer;
				}

				public object Get(IFormatReader parameter)
				{
					var surrogate = _serializer.Get(parameter);
					var result = _provider.GetDeserializedObject(surrogate, _mapping.Origin);
					return result;
				}

				public void Write(IFormatWriter writer, object instance)
				{
					var surrogate = _provider.GetObjectToSerialize(instance, _mapping.Surrogate);
					_serializer.Write(writer, surrogate);
				}
			}
		}
	}
}