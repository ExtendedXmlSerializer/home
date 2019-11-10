using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	sealed class ClassicListNamingExtension : ISerializerExtension
	{
		public static ClassicListNamingExtension Default { get; } = new ClassicListNamingExtension();

		ClassicListNamingExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate<ArrayElement>(IsArraySpecification.Default.Or(IsCollectionTypeSpecification.Default))
			            .Decorate<ITypes, Types>();

		public void Execute(IServices parameter) {}

		sealed class Types : ITypes
		{
			readonly ITypes         _types;
			readonly IIdentityStore _store;

			public Types(ITypes types, IIdentityStore store)
			{
				_types = types;
				_store = store;
			}

			public TypeInfo Get(IIdentity parameter)
				=> _types.Get(parameter) ?? (parameter.Name.StartsWith("ArrayOf")
					                             ? GetTypeInfo(parameter)
					                             : null);

			TypeInfo GetTypeInfo(IIdentity parameter)
			{
				var identity = _store.Get(parameter.Name.Replace("ArrayOf", string.Empty),
				                          parameter.Identifier);
				var typeInfo = _types.Get(identity);
				var result = typeInfo?.MakeArrayType()
				                     .GetTypeInfo();
				return result;
			}
		}

		sealed class ArrayElement : IElement
		{
			readonly IIdentities                _identities;
			readonly IIdentityStore             _store;
			readonly ICollectionItemTypeLocator _locator;

			[UsedImplicitly]
			public ArrayElement(IIdentities identities, IIdentityStore store)
				: this(identities, store, CollectionItemTypeLocator.Default) {}

			public ArrayElement(IIdentities identities, IIdentityStore store, ICollectionItemTypeLocator locator)
			{
				_identities = identities;
				_store      = store;
				_locator    = locator;
			}

			public IWriter Get(TypeInfo parameter)
			{
				var typeInfo = _locator.Get(parameter);
				var element  = _identities.Get(typeInfo);
				var identity = _store.Get($"ArrayOf{element.Name}", element.Identifier ?? string.Empty);
				return new ArrayIdentity(identity).Adapt();
			}
		}

		sealed class ArrayIdentity : IWriter<Array>
		{
			readonly IIdentity _identity;

			public ArrayIdentity(IIdentity identity) => _identity = identity;

			public void Write(IFormatWriter writer, Array instance)
			{
				writer.Start(_identity);
			}
		}
	}
}