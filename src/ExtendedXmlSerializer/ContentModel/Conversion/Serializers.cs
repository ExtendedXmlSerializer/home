using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core.Sources;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	sealed class Serializers : CacheBase<TypeInfo, ISerializer>, ISerializers
	{
		readonly IConverters          _converters;
		readonly IContentReaders      _readers;
		readonly IContentWriters      _writers;
		readonly Content.ISerializers _fallback;

		// ReSharper disable once TooManyDependencies
		public Serializers(IConverters converters, IContentReaders readers, IContentWriters writers,
		                   Content.ISerializers fallback)
		{
			_converters = converters;
			_readers    = readers;
			_writers    = writers;
			_fallback   = fallback;
		}

		protected override ISerializer Create(TypeInfo parameter)
		{
			var converter = _converters.Get(parameter);
			var result = converter != null
				             ? new Serializer(_readers.Get(converter.Parse), _writers.Get(converter.Format))
				             : _fallback.Get(parameter);
			return result;
		}
	}
}