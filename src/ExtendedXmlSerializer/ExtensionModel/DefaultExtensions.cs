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
	public sealed class DefaultExtensions : ItemsBase<ISerializerExtension>
	{
		public static DefaultExtensions Default { get; } = new DefaultExtensions();

		DefaultExtensions()
			: this(DefaultMetadataSpecification.Default, DefaultMemberOrder.Default) {}

		readonly IMetadataSpecification _metadata;
		readonly IParameterizedSource<MemberInfo, int> _defaultMemberOrder;


		public DefaultExtensions(IMetadataSpecification metadata,
		                         IParameterizedSource<MemberInfo, int> defaultMemberOrder)
		{
			_metadata = metadata;
			_defaultMemberOrder = defaultMemberOrder;
		}

		public override IEnumerator<ISerializerExtension> GetEnumerator()
		{
			yield return new DefaultReferencesExtension();
			yield return Contents.Default;
			yield return ContentModelExtension.Default;
			yield return TypeModelExtension.Default;
			yield return SingletonActivationExtension.Default;
			yield return new XmlSerializationExtension();
			yield return new ConvertersExtension();
			yield return MemberModelExtension.Default;
			yield return new MemberNamesExtension();
			yield return new MemberOrderingExtension(_defaultMemberOrder);
			yield return new AllowedMembersExtension(_metadata);
			yield return new AllowedMemberValuesExtension();
			yield return new MemberFormatExtension();
			yield return ImmutableArrayExtension.Default;
			yield return SerializationExtension.Default;
			//yield return new RegisteredSerializersExtension();
			yield return CachingExtension.Default;
		}
	}
}