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
using ExtendedXmlSerializer.Core.Collections;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Services;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GenericTypes = ExtendedXmlSerializer.ContentModel.Reflection.GenericTypes;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	/*class MetadataNames : MetadataTable<string>, IMetadataNames
	{
		public MetadataNames(IDictionary<TypeInfo, string> types) : this(types, new Dictionary<MemberInfo, string>()) {}

		public MetadataNames(IDictionary<TypeInfo, string> types, IDictionary<MemberInfo, string> members)
			: base(new TypedTable<string>(types), new MemberTable<string>(members)) { }
	}*/

	public sealed class TypeResolutionExtension : ISerializerExtension
	{
		public static TypeResolutionExtension Default { get; } = new TypeResolutionExtension();
		TypeResolutionExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Register<IAssemblyTypePartitions, AssemblyTypePartitions>()
			            .Register<ITypeFormatter, TypeFormatter>()
			            .Register<ITypePartResolver, TypePartResolver>()
			            .RegisterInstance<IReadOnlyDictionary<Assembly, IIdentity>>(WellKnownAssemblyIdentities.Default)
			            .RegisterInstance<INamespaceFormatter>(NamespaceFormatter.Default)
			            .Register<IIdentities, Identities>()
			            .Register<IIdentifiers, Identifiers>()
			            .Register<ITypeIdentities, TypeIdentities>()
			            .Register<ITypes, ContentModel.Reflection.Types>()
			            .Register<IGenericTypes, GenericTypes>()
			            .RegisterInstance<IPartitionedTypeSpecification>(PartitionedTypeSpecification.Default);

		void ICommand<IServices>.Execute(IServices parameter) { }
	}

	[Extension(typeof(MetadataNamesExtension))]
	public sealed class RegisteredNamesProperty : MetadataProperty<string>
	{
		public static RegisteredNamesProperty Default { get; } = new RegisteredNamesProperty();
		RegisteredNamesProperty() {}
	}

	sealed class MetadataNames : LinkedDecoratedSource<MemberInfo, string>, INames
	{
		public MetadataNames(MetadataPropertyReference<RegisteredNamesProperty, string> table, INames next) : base(table, next) {}
	}

	public sealed class MetadataNamesExtension : ISerializerExtension
	{
		readonly ISerializerExtension _format;

		public MetadataNamesExtension(ISerializerExtension format) => _format = format;

		public IServiceRepository Get(IServiceRepository parameter)
			=> _format.Get(parameter)
			          .DecorateWithDependencies<INames, MetadataNames>()
			          .Decorate<IRegisteredTypes, RegisteredTypes>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}

	sealed class RegisteredTypes : Items<TypeInfo>, IRegisteredTypes
	{
		public RegisteredTypes(IRegisteredTypes types, IMetadataTable table)
			: base(types.Union(table.Select(x => x.Get()).OfType<TypeInfo>())) {}
	}
}