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

using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Collections;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Content.Registration;
using ExtendedXmlSerializer.ExtensionModel.References;
using ExtendedXmlSerializer.ExtensionModel.Services;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel
{
	sealed class DefaultExtensions : ItemsBase<IGroup<ISerializerExtension>>
	{
		public static DefaultExtensions Default { get; } = new DefaultExtensions();
		DefaultExtensions() : this(DefaultMetadataSpecification.Default, DefaultMemberOrder.Default,
		                           Xml.MetadataNamesExtension.Default) {}

		readonly IMetadataSpecification                _metadata;
		readonly IParameterizedSource<MemberInfo, int> _defaultMemberOrder;
		readonly ISerializerExtension _names;

		public DefaultExtensions(IMetadataSpecification metadata,
		                         IParameterizedSource<MemberInfo, int> defaultMemberOrder, ISerializerExtension names)
		{
			_metadata           = metadata;
			_defaultMemberOrder = defaultMemberOrder;
			_names = names;
		}

		public override IEnumerator<IGroup<ISerializerExtension>> GetEnumerator()
		{
			var all = new KeyedByTypeCollection<ISerializerExtension>();
			yield return new ExtensionGroup(Categories.Start, all,
			                                new ConfigurationServicesExtension(),
			                                ExtensionServicesExtension.Default
			                               );

			yield return new ExtensionGroup(Categories.ReflectionModel, all,
			                                TypeModelExtension.Default,
			                                SingletonActivationExtension.Default,
			                                TypeResolutionExtension.Default,
			                                new Types.MetadataNamesExtension(_names)
			                               );

			yield return new ExtensionGroup(Categories.ContentModel, all,
			                                ContentModelExtension.Default/*,
			                                MemberModelExtension.Default,
			                                new MemberPolicyExtension(_metadata),
			                                new AllowedMemberValuesExtension(),
			                                new MemberNamesExtension(),
			                                new MemberOrderingExtension(_defaultMemberOrder)*/
			                               );

			yield return new ExtensionGroup(Categories.ObjectModel, all,
			                                new DefaultReferencesExtension());
			yield return new ExtensionGroup(Categories.Framework, all,
			                                SerializationExtension.Default);
			yield return new ExtensionGroup(Categories.Elements, all,
			                                ElementsExtension.Default/*,
											VariableTypeElementsExtension.Default,
			                                GenericElementsExtension.Default,
			                                ArrayElementsExtension.Default*/

			                                );
			yield return new ExtensionGroup(Categories.Content, all,
			                                DefaultContentsExtension.Default,
											EnumerationContentsExtension.Default,
			                                NullableStructureContentsExtension.Default
											/*MemberedContentsExtension.Default,
											CollectionContentsExtension.Default,
											ArrayContentsExtension.Default,
											DictionaryContentsExtension.Default,*/
			                                /*
			                                
			                                ,
			                                ImmutableArrayContentsExtension.Default*/
			                               );

			yield return new ExtensionGroup(Categories.Registrations, all,
			                                RegisteredContentsExtension.Default
			                               );
			yield return new ExtensionGroup(Categories.Format, all,
			                                new XmlSerializationExtension()/*,
			                                new MemberFormatExtension()*/
			                               );
			yield return new ExtensionGroup(Categories.Caching, all, CachingExtension.Default);
			yield return new ExtensionGroup(Categories.Finish, all);
		}
	}
}