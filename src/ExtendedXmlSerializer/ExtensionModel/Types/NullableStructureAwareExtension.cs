using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
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
				var serializer = _previous.Get(underlying != null ? underlying : parameter);
				return serializer;
			}
		}
	}
}
