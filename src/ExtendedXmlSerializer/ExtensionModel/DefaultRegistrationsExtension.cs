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
using System.Linq;
using ExtendedXmlSerialization.ContentModel;
using ExtendedXmlSerialization.ContentModel.Collections;
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.ContentModel.Xml;
using ExtendedXmlSerialization.Core;
using RuntimeSerializer = ExtendedXmlSerialization.ContentModel.RuntimeSerializer;

namespace ExtendedXmlSerialization.ExtensionModel
{
	class DefaultRegistrationsExtension : ISerializerExtension
	{
		public IServiceRepository Get(IServiceRepository parameter)
		{
			return parameter.RegisterInstance<IActivation>(Activation.Default)
			                .Register<ISerializer, RuntimeSerializer>()
			                .Register<IXmlFactory, XmlFactory>()
			                .Register<IDictionaryEntries, DictionaryEntries>()
			                .Register<ArrayContentOption>()
			                .Register<DictionaryContentOption>()
			                .Register<CollectionContentOption>()
			                .Register<MemberedContentOption>()
			                .Register<RuntimeContentOption>()
			                .Register<IEnumerable<IContentOption>, ContentOptions>()
			                .Register(provider => provider.GetAllInstances<IContentOption>().ToArray())
			                .Register<IMetadataSpecification, MetadataSpecification>()
			                .Register<IValidMemberSpecification, AllowsAccessSpecification>()
			                .Register<ITypeMemberSource, TypeMemberSource>()
			                .Register<ITypeMembers, TypeMembers>()
			                .Register<IMembers, Members>()
			                .Register<IMemberAccessors, MemberAccessors>()
			                .Register<WritableMemberAccessors>()
			                .Register<ReadOnlyCollectionAccessors>()
			                .Register<VariableTypeMemberContents>()
			                .Register<DefaultMemberContents>()
			                .Register<IMemberContents, MemberContents>()
			                .Register<IMemberSerializers, MemberSerializers>()
			                .Register<IMemberSerializations, MemberSerializations>()
			                .Register<ContainsStaticReferenceSpecification>()
			                .Register<IStaticReferenceSpecification, ContainsStaticReferenceSpecification>()
			                .Register<IRootReferences, RootReferences>()
			                .Register<IExtendedXmlSerializer, ExtendedXmlSerializer>();
		}

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}