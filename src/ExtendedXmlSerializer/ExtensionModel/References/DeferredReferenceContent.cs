using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ExtensionModel.Content;
using System.Collections;
using System.Linq;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class DeferredReferenceContent : ContentModel.ISerializer
	{
		readonly IReservedItems  _reserved;
		readonly IRootReferences _references;
		readonly ContentModel.ISerializer     _serializer;

		public DeferredReferenceContent(IRootReferences references, ContentModel.ISerializer serializer)
			: this(ReservedItems.Default, references, serializer) {}

		public DeferredReferenceContent(IReservedItems reserved, IRootReferences references, ContentModel.ISerializer serializer)
		{
			_reserved   = reserved;
			_references = references;
			_serializer = serializer;
		}

		public object Get(IFormatReader parameter) => _serializer.Get(parameter);

		public void Write(IFormatWriter writer, object instance)
		{
			var list = instance as IList;
			if (list != null)
			{
				var references = _references.Get(writer);
				var hold       = _reserved.Get(writer);
				foreach (var item in list)
				{
					if (references.Contains(item))
					{
						hold.Get(item).Push(list);
					}
				}
			}

			_serializer.Write(writer, instance);
		}
	}
}