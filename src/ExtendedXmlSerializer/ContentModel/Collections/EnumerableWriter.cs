using System.Collections;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Collections
{
	sealed class EnumerableWriter : IWriter<IEnumerable>
	{
		readonly IEnumerators _enumerator;
		readonly IWriter      _item;

		public EnumerableWriter(IEnumerators enumerators, IWriter item)
		{
			_enumerator = enumerators;
			_item       = item;
		}

		public void Write(IFormatWriter writer, IEnumerable instance)
		{
			var iterator = _enumerator.Get(instance);
			while (iterator.MoveNext())
			{
				_item.Write(writer, iterator.Current);
			}
		}
	}
}