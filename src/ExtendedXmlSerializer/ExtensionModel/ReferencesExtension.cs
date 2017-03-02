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
using ExtendedXmlSerialization.ContentModel;
using ExtendedXmlSerialization.ContentModel.Content;
using LightInject;

namespace ExtendedXmlSerialization.ExtensionModel
{
	class ReferencesExtension : ISerializerExtension
	{
		readonly IEntityMembers _members;
		readonly Func<IServiceFactory, IActivation, IActivation> _decorate;

		public ReferencesExtension() : this(new EntityMembers(new Dictionary<TypeInfo, MemberInfo>())) {}

		public ReferencesExtension(IEntityMembers members)
		{
			_members = members;
			_decorate = Decorate;
		}

		public IServices Get(IServices parameter) =>
			parameter.RegisterInstance(_members)
			         .Register<IStoredEncounters, StoredEncounters>()
			         .Register<IEntities, Entities>()
			         .Register<ReferencesContentAlteration>()
			         .Decorate(_decorate)
			         .Decorate<IContentOptions>(
				         (factory, options) =>
					         new AlteredContentOptions(options, factory.GetInstance<ReferencesContentAlteration>()));

		IActivation Decorate(IServiceFactory factory, IActivation activation)
			=> new ReferenceActivation(activation, factory.GetInstance<IEntities>());
	}
}