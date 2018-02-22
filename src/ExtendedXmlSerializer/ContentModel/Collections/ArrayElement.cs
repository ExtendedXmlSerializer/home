using System;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Collections {
	sealed class ArrayElement : IElements
	{
		readonly ICollectionItemTypeLocator _locator;
		readonly IWriter<Array> _identity;

		public ArrayElement(IIdentities identities)
			: this(new Identity<Array>(identities.Get(Support<Array>.Key)), CollectionItemTypeLocator.Default)
		{
		}

		public ArrayElement(IWriter<Array> identity, ICollectionItemTypeLocator locator)
		{
			_locator = locator;
			_identity = identity;
		}

		public IWriter Get(TypeInfo parameter)
			=> new ArrayIdentity(_identity, _locator.Get(parameter)).Adapt();
	}
}