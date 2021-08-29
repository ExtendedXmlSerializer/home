using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Format;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	sealed class NullableStructureAwareExtension : ISerializerExtension
	{
		public static NullableStructureAwareExtension Default { get; } = new NullableStructureAwareExtension();

		NullableStructureAwareExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.DecorateContentsWith<NullableStructureAwareContents>().Then();

		public void Execute(IServices parameter) {}

		sealed class NullableStructureAwareContents : IContents
		{
			readonly IConverters _converters;
			readonly IContents   _previous;

			public NullableStructureAwareContents(IConverters converters, IContents previous)
			{
				_converters = converters;
				_previous   = previous;
			}

			public ISerializer Get(TypeInfo parameter)
			{
				var underlying = Nullable.GetUnderlyingType(parameter);
				if (underlying != null)
				{
					var serializer = _previous.Get(_converters.Get(parameter) != null ? parameter : underlying);
					return new Serializer(new Reader(serializer), serializer);
				}

				return _previous.Get(parameter);
			}
		}

		sealed class Reader : IReader
		{
			readonly IReader _previous;

			public Reader(IReader previous) => _previous = previous;

			public object Get(IFormatReader parameter)
			{
				try
				{
					return _previous.Get(parameter);
				}
				catch (FormatException)
				{
					return null;
				}
			}
		}
	}
}