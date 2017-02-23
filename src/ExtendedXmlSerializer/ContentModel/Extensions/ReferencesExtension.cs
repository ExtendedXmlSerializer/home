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

using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerialization.Configuration;
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.ContentModel.Xml;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.ContentModel.Extensions
{
	class ReferencesExtension : ISerializerExtension
	{
		readonly IDictionary<TypeInfo, IEntity> _store;

		public ReferencesExtension() : this(new Dictionary<TypeInfo, IEntity>()) {}

		public ReferencesExtension(IDictionary<TypeInfo, IEntity> store)
		{
			_store = store;
		}

		public IServiceRepository Get(IServiceRepository parameter)
		{
			var current = parameter.GetValid<IContentOptions>();
			var entities = new Entities(_store);
			var identities = new Identities(parameter.GetValid<IMembers>(), entities);
			var altered = new AlteredContentOptions(current,
			                                        new ReferencesContentAlteration(parameter.GetValid<ITypeSelector>(),
			                                                                        identities, entities));
			var result = parameter.Replace<IContentOptions>(altered);
			return result;
		}

		SerializationResult IParameterizedSource<SerializationResult, SerializationResult>.Get(SerializationResult parameter)
			=> parameter;
	}
}