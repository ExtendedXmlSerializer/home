using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer
{
	sealed class Serializer<TRead, TWrite> : ISerializer<TRead, TWrite>
	{
		readonly IRead<TRead>   _read;
		readonly IWrite<TWrite> _write;

		public Serializer(IRead<TRead> read, IWrite<TWrite> write)
		{
			_read  = read;
			_write = write;
		}

		public void Serialize(TWrite writer, object instance)
		{
			_write.Execute(new Writing<TWrite>(writer, instance));
		}

		public object Deserialize(TRead reader) => _read.Get(reader);
	}
}