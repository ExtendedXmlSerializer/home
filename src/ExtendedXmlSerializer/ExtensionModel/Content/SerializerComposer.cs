using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.Core.Sources;
using System;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	class SerializerComposer<T> : ISerializerComposer
	{
		readonly Func<ISerializer<T>, ISerializer<T>> _select;

		public SerializerComposer(Func<ISerializer<T>, ISerializer<T>> select) => _select = select;

		public ISerializer Get(ISerializer parameter) => _select(parameter.For<T>())
			.Adapt();
	}

	class SerializerComposer : DelegatedAlteration<ISerializer>, ISerializerComposer
	{
		public SerializerComposer(Func<ISerializer, ISerializer> alteration) : base(alteration) {}
	}
}