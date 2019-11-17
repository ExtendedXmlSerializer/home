using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core.Sources;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class DeferredReferenceSerializers : CacheBase<TypeInfo, ContentModel.ISerializer>, ISerializers
	{
		readonly ISerializers _serializers;

		public DeferredReferenceSerializers(ISerializers serializers)
		{
			_serializers = serializers;
		}

		protected override ContentModel.ISerializer Create(TypeInfo parameter)
			=> new DeferredReferenceSerializer(_serializers.Get(parameter));
	}
}