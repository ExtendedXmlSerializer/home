using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class Enclosure : Enclosure<object>
	{
		public Enclosure(IWriter<object> start, IWriter<object> body) : base(start, body) {}
	}

	class Enclosure<T> : IWriter<T>
	{
		readonly IWriter<T> _start;
		readonly IWriter<T> _body;
		readonly IWriter<T> _finish;

		public Enclosure(IWriter<T> start, IWriter<T> body) : this(start, body, EndCurrentElement<T>.Default) {}

		public Enclosure(IWriter<T> start, IWriter<T> body, IWriter<T> finish)
		{
			_start  = start;
			_body   = body;
			_finish = finish;
		}

		public void Write(IFormatWriter writer, T instance)
		{
			_start.Write(writer, instance);
			_body.Write(writer, instance);
			_finish.Write(writer, instance);
		}
	}
}