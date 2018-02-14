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
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Services;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	public interface IMetadataNames : IMemberTable<string> {}

	class MetadataNames : ReflectionModel.MetadataTable<string>, IMetadataNames
	{
		public MetadataNames(IDictionary<TypeInfo, string> types)
			: this(types, new Dictionary<MemberInfo, string>()) {}

		public MetadataNames(IDictionary<TypeInfo, string> types, IDictionary<MemberInfo, string> members)
			: base(new TypedTable<string>(types), new MemberTable<string>(members)) { }
	}

	public sealed class MemberNamesExtension : ISerializerExtension, IAssignable<MemberInfo, string>, IParameterizedSource<MemberInfo, string>
	{
		readonly IMetadataNames _names;

		public MemberNamesExtension() : this(new MetadataNames(DefaultNames.Default)) {}

		public MemberNamesExtension(IMetadataNames names) => _names = names;

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterInstance<INames>(new Names(_names.Or(DeclaredNames.Default).Or(DeclaredMemberNames.Default)))
			            .Register<IAssemblyTypePartitions, AssemblyTypePartitions>()
			            .Register<ITypeFormatter, TypeFormatter>()
			            .Register<ITypePartResolver, TypePartResolver>()
			            .RegisterInstance<IReadOnlyDictionary<Assembly, IIdentity>>(WellKnownIdentities.Default)
			            .RegisterInstance<INamespaceFormatter>(NamespaceFormatter.Default)
			            .Register<IIdentities, Identities>()
			            .Register<IIdentifiers, Identifiers>()
			            .Register<ITypeIdentities, TypeIdentities>()
			            .Register<ContentModel.Reflection.ITypes, ContentModel.Reflection.Types>()
			            .Register<IGenericTypes, GenericTypes>()
			            .RegisterInstance<IPartitionedTypeSpecification>(PartitionedTypeSpecification.Default);

		void ICommand<IServices>.Execute(IServices parameter) {}

		public void Execute(KeyValuePair<MemberInfo, string> parameter)
		{
			_names.Execute(parameter);
		}

		public string Get(MemberInfo parameter) => _names.Get(parameter);
	}
}