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
using ExtendedXmlSerialization.ContentModel;
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.ContentModel.Properties;
using ExtendedXmlSerialization.ContentModel.Xml;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.ExtensionModel;
using ExtendedXmlSerialization.TypeModel;
using Elements = ExtendedXmlSerialization.ContentModel.Content.Elements;
using Identities = ExtendedXmlSerialization.ContentModel.Xml.Identities;
using IIdentities = ExtendedXmlSerialization.ContentModel.Xml.IIdentities;
using TypeFormatter = ExtendedXmlSerialization.ContentModel.Xml.TypeFormatter;

namespace ExtendedXmlSerialization.Configuration
{
	public class TypeNamesExtension : TypedTable<string>, ISerializerExtension
	{
		readonly static DefaultNames DefaultNames = DefaultNames.Default;

		readonly IDictionary<TypeInfo, string> _names;

		public TypeNamesExtension() : this(DefaultNames) {}

		public TypeNamesExtension(IDictionary<TypeInfo, string> names)
		{
			_names = names;
		}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Register<ITypeFormatters, TypeFormatters>()
			            .Register<ITypeParsers, TypeParsers>()
			            .Register<IAssemblyTypePartitions, AssemblyTypePartitions>()
			            .Register<IElements, Elements>()
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
			            .RegisterInstance(_names)
			            .Register<IAliases, TypeAliases>()
			            .Decorate<IAliases>(Register)
			            .Decorate<IAliases>((provider, aliases) => new Aliases(aliases.Or(MetadataAliases.Default)));

		IAliases Register(IServiceProvider provider, IAliases aliases) => new Aliases(this.Or(aliases));

		public void Execute(IServices parameter) {}
	}
}