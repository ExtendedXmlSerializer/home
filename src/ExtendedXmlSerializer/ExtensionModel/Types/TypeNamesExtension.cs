using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	/// <summary>
	/// Default extension that tracks registrations for type names, as well as configuration of necessary components for
	/// successful type name resolution.
	/// </summary>
	public sealed class TypeNamesExtension : ISerializerExtension
	{
		readonly static IDictionary<TypeInfo, string> Defaults = DefaultNames.Default;

		readonly IDictionary<TypeInfo, string> _defaults;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public TypeNamesExtension() : this(new Dictionary<TypeInfo, string>(), Defaults) {}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="names"></param>
		/// <param name="defaults"></param>
		public TypeNamesExtension(IDictionary<TypeInfo, string> names, IDictionary<TypeInfo, string> defaults)
		{
			Names     = names;
			_defaults = defaults;
		}

		/// <summary>
		/// The current store of name registrations.
		/// </summary>
		public IDictionary<TypeInfo, string> Names { get; }

		/// <inheritdoc />
		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterInstance(_defaults)
			            .Register<IAssemblyTypePartitions, AssemblyTypePartitions>()
			            .Register<ITypeFormatter, TypeFormatter>()
			            .Register<ITypePartResolver, TypePartResolver>()
			            .RegisterInstance<IReadOnlyDictionary<Assembly, IIdentity>>(WellKnownIdentities.Default)
			            .RegisterInstance<INamespaceFormatter>(NamespaceFormatter.Default)
			            .Register<IIdentities, Identities>()
			            .Register<IIdentifiers, Identifiers>()
			            .Register<ITypeIdentities, TypeIdentities>()
			            .Register<ITypes, ContentModel.Reflection.Types>()
			            .Register<IGenericTypes, GenericTypes>()
			            .RegisterInstance<IPartitionedTypeSpecification>(PartitionedTypeSpecification.Default)
			            .RegisterInstance<INames>(new Names(new TypedTable<string>(Names).Or(DeclaredNames.Default)
			                                                                             .Or(new TypedTable<string
			                                                                                 >(_defaults))));

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}