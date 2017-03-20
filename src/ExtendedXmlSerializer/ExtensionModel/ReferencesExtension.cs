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

using System;
using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.TypeModel;

namespace ExtendedXmlSerializer.ExtensionModel
{
	class ReferencesExtension : TypedTable<MemberInfo>, IEntityMembers, ISerializerExtension
	{
		readonly Func<System.IServiceProvider, IActivation, IActivation> _decorate;

		public ReferencesExtension() : this(new Dictionary<TypeInfo, MemberInfo>()) {}

		public ReferencesExtension(IDictionary<TypeInfo, MemberInfo> store) : base(store)
		{
			_decorate = Decorate;
		}

		public IServiceRepository Get(IServiceRepository parameter) =>
			parameter.RegisterInstance<IEntityMembers>(this)
			         .Register<IStoredEncounters, StoredEncounters>()
			         .Register<IEntities, Entities>()
			         .Decorate(_decorate)
			         .Decorate<IContents>((factory, contents) =>
				                              new ReferenceContents(factory.Get<IStoredEncounters>(), factory.Get<IEntities>(),
				                                                    contents))
			         .Decorate<ISerializers>((factory, serializers) => new CircularReferenceEnabledSerialization(serializers))
			         .Decorate<IContents>((factory, contents) => new RecursionAwareContents(contents));

		static IActivation Decorate(System.IServiceProvider factory, IActivation activation)
			=> new ReferenceActivation(activation, factory.Get<IEntities>());

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}