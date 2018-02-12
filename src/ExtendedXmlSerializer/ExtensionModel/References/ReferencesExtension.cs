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

using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Services;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	[Dependency(typeof(RootInstanceExtension))]
	sealed class ReferencesExtension : TypedTable<MemberInfo>, IEntityMembers, ISerializerExtension
	{
		public ReferencesExtension() : this(new Dictionary<TypeInfo, MemberInfo>()) {}

		public ReferencesExtension(IDictionary<TypeInfo, MemberInfo> store) : base(store) {}

		public IServiceRepository Get(IServiceRepository parameter) =>
			parameter.Register<IRootReferences, RootReferences>()
			         .RegisterInstance<IEntityMembers>(this)
			         .RegisterInstance<IReferenceMaps>(ReferenceMaps.Default)
			         .Register<IReferenceEncounters, ReferenceEncounters>()
			         .Register<IEntities, Entities>()
			         .Decorate<IActivation, ReferenceActivation>()
			         .Decorate<ContentModel.Content.ISerializers, CircularReferenceEnabledSerialization>()
			         .Decorate<IContents, ReferenceContents>()
			         .Decorate<IContents, RecursionAwareContents>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}