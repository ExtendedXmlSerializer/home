using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class DeferredReferenceSerializers : CacheBase<TypeInfo, ISerializer>, ISerializers
	{
		readonly ISerializers _serializers;

		public DeferredReferenceSerializers(ISerializers serializers)
		{
			_serializers = serializers;
		}

		protected override ISerializer Create(TypeInfo parameter)
			=> new DeferredReferenceSerializer(_serializers.Get(parameter));
	}
}