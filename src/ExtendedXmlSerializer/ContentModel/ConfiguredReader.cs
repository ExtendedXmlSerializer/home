using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ContentModel
{
	sealed class ConfiguredReader<T> : IReader<T>
	{
		readonly IReader<T>              _reader;
		readonly ICommand<IFormatReader> _configuration;

		public ConfiguredReader(IReader<T> reader, ICommand<IFormatReader> configuration)
		{
			_reader        = reader;
			_configuration = configuration;
		}

		public T Get(IFormatReader parameter)
		{
			var result = _reader.Get(parameter);
			_configuration.Execute(parameter);
			return result;
		}
	}
}