using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	sealed class Serializers : DecoratedSource<TypeInfo, ISerializer>, ISerializers
	{
		public Serializers(IEnhancer enhancer, ISerializers serializers)
			: base(new AlteredSource<TypeInfo, ISerializer>(enhancer, serializers)) {}
	}
}