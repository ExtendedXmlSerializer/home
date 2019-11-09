using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	sealed class ConverterContents : DelegatedSource<TypeInfo, ISerializer>, IContents
	{
		public ConverterContents(ISerializers serializers) : base(serializers.Get) {}
	}
}