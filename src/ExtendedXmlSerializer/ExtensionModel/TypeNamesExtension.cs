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
using System.Reflection;
using ExtendedXmlSerialization.Configuration;
using ExtendedXmlSerialization.ContentModel;
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.ContentModel.Properties;
using ExtendedXmlSerialization.ContentModel.Xml;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.TypeModel;
using Identities = ExtendedXmlSerialization.ContentModel.Xml.Identities;
using IIdentities = ExtendedXmlSerialization.ContentModel.Xml.IIdentities;
using TypeFormatter = ExtendedXmlSerialization.ContentModel.Xml.TypeFormatter;

namespace ExtendedXmlSerialization.ExtensionModel
{
	sealed class TypeNamesExtension : ISerializerExtension
	{
		readonly static IDictionary<TypeInfo, string> Defaults = DefaultNames.Default;

		readonly IDictionary<TypeInfo, string> _defaults;

		public TypeNamesExtension() : this(new Dictionary<TypeInfo, string>(), Defaults) {}

		public TypeNamesExtension(IDictionary<TypeInfo, string> names, IDictionary<TypeInfo, string> defaults)
		{
			Names = names;
			_defaults = defaults;
		}

		public IDictionary<TypeInfo, string> Names { get; }

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter
				.RegisterInstance(_defaults)
				.Register<ITypeFormatters, TypeFormatters>()
				.Register<ITypeParsers, TypeParsers>()
				.Register<IAssemblyTypePartitions, AssemblyTypePartitions>()
				.Register<IElements, ContentModel.Content.Elements>()
				.Register<ITypeFormatter, TypeFormatter>()
				.Register<ITypeFormatters, TypeFormatters>()
				.Register<ITypeParsers, TypeParsers>()
				.Register<IIdentities, Identities>()
				.Register<ITypeIdentities, TypeIdentities>()
				.Register<IGenericTypes, GenericTypes>()
				.Register<ITypes, Types>()
				.Register<IContainsAliasSpecification, ContainsAliasSpecification>()
				.Register<IArgumentsProperty, ArgumentsProperty>()
				.Register<IItemTypeProperty, ItemTypeProperty>()
				.Register<ITypeProperty, TypeProperty>()
				.Register(Register);

		INames Register(IServiceProvider provider)
			=> new Names(new TypedTable<string>(Names)
				             .Or(DeclaredNames.Default)
				             .Or(new TypedTable<string>(_defaults)));

		public void Execute(IServices parameter) {}
	}
}