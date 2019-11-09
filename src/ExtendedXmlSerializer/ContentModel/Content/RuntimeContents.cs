using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class RuntimeContents : FixedInstanceSource<TypeInfo, ISerializer>, IContents
	{
		public RuntimeContents(ISerializer instance) : base(instance) {}
	}
}