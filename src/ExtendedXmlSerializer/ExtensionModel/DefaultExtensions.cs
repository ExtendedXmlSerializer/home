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

using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Collections;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Content.Members;
using ExtendedXmlSerializer.ExtensionModel.References;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel
{
	sealed class DefaultGroups : ItemsBase<IGroup<ISerializerExtension>>
	{
		public static GroupName Start = new GroupName("Start"),
		                        TypeSystem = new GroupName("Type System"),
		                        Framework = new GroupName("Framework"),
		                        Elements = new GroupName("Elements"),
		                        Content = new GroupName("Content"),
		                        Format = new GroupName("Format"),
		                        Caching = new GroupName("Caching"),
		                        Finish = new GroupName("Finish");

		public static DefaultGroups Default { get; } = new DefaultGroups();
		DefaultGroups() : this(DefaultMetadataSpecification.Default, DefaultMemberOrder.Default) {}

		readonly IMetadataSpecification _metadata;
		readonly IParameterizedSource<MemberInfo, int> _defaultMemberOrder;

		public DefaultGroups(IMetadataSpecification metadata,
		                     IParameterizedSource<MemberInfo, int> defaultMemberOrder)
		{
			_metadata = metadata;
			_defaultMemberOrder = defaultMemberOrder;
		}

		public override IEnumerator<IGroup<ISerializerExtension>> GetEnumerator()
		{
			yield return new Group<ISerializerExtension>(Start,
														 new DefaultReferencesExtension()
			                                             );

			yield return new Group<ISerializerExtension>(TypeSystem,
			                                             TypeModelExtension.Default,
			                                             SingletonActivationExtension.Default,
														 new MemberNamesExtension(),
			                                             new MemberOrderingExtension(_defaultMemberOrder),
			                                             ImmutableArrayExtension.Default,
			                                             MemberModelExtension.Default
														 );
			yield return new Group<ISerializerExtension>(Framework,
			                                             SerializationExtension.Default);
			yield return new Group<ISerializerExtension>(Elements);
			yield return new Group<ISerializerExtension>(Content,
														 Contents.Default,
			                                             ContentModelExtension.Default,
			                                             new AllowedMembersExtension(_metadata),
			                                             new AllowedMemberValuesExtension(),
			                                             new ConvertersExtension()
														 //new RegisteredSerializersExtension(),
														 );
			yield return new Group<ISerializerExtension>(Format,
			                                             new XmlSerializationExtension(),
														 new MemberFormatExtension()
														 );
			yield return new Group<ISerializerExtension>(Caching, CachingExtension.Default);
			yield return new Group<ISerializerExtension>(Finish);
		}
	}

	public sealed class DefaultExtensions : GroupContainer<ISerializerExtension>
	{
		public static DefaultExtensions Default { get; } = new DefaultExtensions();
		DefaultExtensions() : base(DefaultGroups.Default) {}
	}
}