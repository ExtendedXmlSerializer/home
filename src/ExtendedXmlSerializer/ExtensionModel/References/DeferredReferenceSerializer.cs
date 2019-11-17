using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ExtensionModel.Content;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class DeferredReferenceSerializer : ContentModel.ISerializer
	{
		readonly IReservedItems _reserved;
		readonly ContentModel.ISerializer    _serializer;

		public DeferredReferenceSerializer(ContentModel.ISerializer serializer) : this(ReservedItems.Default, serializer) {}

		public DeferredReferenceSerializer(IReservedItems reserved, ContentModel.ISerializer serializer)
		{
			_reserved   = reserved;
			_serializer = serializer;
		}

		public object Get(IFormatReader parameter) => _serializer.Get(parameter);

		public void Write(IFormatWriter writer, object instance)
		{
			var lists = _reserved.Get(writer);
			foreach (var o in Yield(instance))
			{
				var reserved = lists.Get(o);
				if (reserved.Any())
				{
					reserved.Pop();
				}
			}

			_serializer.Write(writer, instance);
		}

		static IEnumerable<object> Yield(object instance)
		{
			if (instance is DictionaryEntry)
			{
				var entry = (DictionaryEntry)instance;
				yield return entry.Key;
				yield return entry.Value;
			}
			else
			{
				yield return instance;
			}
		}
	}
}