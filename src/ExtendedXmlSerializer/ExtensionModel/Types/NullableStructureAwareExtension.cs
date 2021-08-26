using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
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
			readonly IContents _previous;

			public NullableStructureAwareContents(IContents previous) => _previous = previous;

			public ISerializer Get(TypeInfo parameter)
			{
				var underlying = Nullable.GetUnderlyingType(parameter);
				var decorate   = underlying != null;
				var serializer = _previous.Get(decorate ? underlying : parameter);
				var result     = decorate ? new Serializer(new Reader(serializer), serializer) : serializer;
				return result;
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
