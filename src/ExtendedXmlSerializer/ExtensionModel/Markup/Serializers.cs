using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core.Sources;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	sealed class Serializers : DecoratedSource<TypeInfo, ContentModel.ISerializer>, ISerializers
	{
		public Serializers(IEnhancer enhancer, ISerializers serializers)
			: base(new AlteredSource<TypeInfo, ContentModel.ISerializer>(enhancer, serializers)) {}
	}
}