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

using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Services;
using System.Reflection;
using System.Xml.Serialization;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	public sealed class DeclaredMemberNames : INames
	{
		public static DeclaredMemberNames Default { get; } = new DeclaredMemberNames();
		DeclaredMemberNames() {}

		public string Get(MemberInfo parameter)
			=> parameter.GetCustomAttribute<XmlAttributeAttribute>(false)?.AttributeName.NullIfEmpty() ??
			   DefaultXmlElementAttribute.Default.Get(parameter)?.ElementName.NullIfEmpty();
	}

	public sealed class MetadataNamesExtension : ISerializerExtension
	{
		public static MetadataNamesExtension Default { get; } = new MetadataNamesExtension();
		MetadataNamesExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterInstance<INames>(new Names(DeclaredNames.Default.Or(DeclaredMemberNames.Default)))
			            .RegisterInstance<IRegisteredTypes>(RegisteredTypes.Default);

		void ICommand<IServices>.Execute(IServices parameter) {}
	}

	sealed class RegisteredTypes : Items<TypeInfo>, IRegisteredTypes
	{
		public static RegisteredTypes Default { get; } = new RegisteredTypes();
		RegisteredTypes() : base(WellKnownAliases.Default.Keys) {}
	}

}