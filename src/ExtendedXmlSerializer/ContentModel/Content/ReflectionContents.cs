using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class ReflectionContents : FixedInstanceSource<TypeInfo, ISerializer>, IContents
	{
		public ReflectionContents(ReflectionSerializer serializer) : base(serializer.Adapt()) {}
	}
}