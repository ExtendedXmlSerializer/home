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
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Content.Members;
using ExtendedXmlSerializer.ExtensionModel.References;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ExtensionModel.Xml;

namespace ExtendedXmlSerializer.ExtensionModel
{
	public sealed class DefaultExtensions : ItemsBase<ISerializerExtension>
	{
		public static DefaultExtensions Default { get; } = new DefaultExtensions();
		DefaultExtensions() {}

		public override IEnumerator<ISerializerExtension> GetEnumerator()
		{
			yield return DefaultReferencesExtension.Default;
			yield return ContentModelExtension.Default;
			yield return TypeModelExtension.Default;
			yield return new XmlSerializationExtension();
			yield return new ConverterRegistryExtension();
			yield return MemberModelExtension.Default;
			yield return new TypeNamesExtension();
			yield return new MemberPropertiesExtension();
			yield return new AllowedMembersExtension();
			yield return new AllowedMemberValuesExtension();
			yield return new MemberFormatExtension();
			yield return SerializationExtension.Default;
		}
	}
}