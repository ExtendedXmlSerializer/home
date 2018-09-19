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

using System;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	sealed class ClassicListNamingExtension : ISerializerExtension
	{
		public static ClassicListNamingExtension Default { get; } = new ClassicListNamingExtension();

		ClassicListNamingExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate<ArrayElement>(IsArraySpecification.Default.Or(IsCollectionTypeSpecification.Default))
			            .Decorate<ITypes, Types>();

		public void Execute(IServices parameter) {}

		sealed class Types : ITypes
		{
			readonly ITypes         _types;
			readonly IIdentityStore _store;

			public Types(ITypes types, IIdentityStore store)
			{
				_types = types;
				_store = store;
			}

			public TypeInfo Get(IIdentity parameter)
				=> _types.Get(parameter) ?? (parameter.Name.StartsWith("ArrayOf")
					                             ? GetTypeInfo(parameter)
					                             : null);

			TypeInfo GetTypeInfo(IIdentity parameter)
			{
				var identity = _store.Get(parameter.Name.Replace("ArrayOf", string.Empty),
				                          parameter.Identifier);
				var typeInfo = _types.Get(identity);
				var result = typeInfo?.MakeArrayType().GetTypeInfo();
				return result;
			}
		}

		sealed class ArrayElement : IElement
		{
			readonly IIdentities                _identities;
			readonly IIdentityStore             _store;
			readonly ICollectionItemTypeLocator _locator;

			public ArrayElement(IIdentities identities, IIdentityStore store)
				: this(identities, store, CollectionItemTypeLocator.Default) {}

			public ArrayElement(IIdentities identities, IIdentityStore store, ICollectionItemTypeLocator locator)
			{
				_identities = identities;
				_store      = store;
				_locator    = locator;
			}

			public IWriter Get(TypeInfo parameter)
			{
				var typeInfo = _locator.Get(parameter);
				var element  = _identities.Get(typeInfo);
				var identity = _store.Get($"ArrayOf{element.Name}", element.Identifier ?? string.Empty);
				return new ArrayIdentity(identity).Adapt();
			}
		}

		sealed class ArrayIdentity : IWriter<Array>
		{
			readonly IIdentity _identity;

			public ArrayIdentity(IIdentity identity) => _identity = identity;

			public void Write(IFormatWriter writer, Array instance)
			{
				writer.Start(_identity);
			}
		}
	}
}