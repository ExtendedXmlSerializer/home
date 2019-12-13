using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.Core.Sources;
using JetBrains.Annotations;
using System;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	sealed class SerializerComposer<T> : ISerializerComposer
	{
		readonly Func<ISerializer<T>, ISerializer<T>> _select;

		[UsedImplicitly]
		public SerializerComposer(ISerializerComposer<T> composer) : this(composer.Get) {}

		public SerializerComposer(Func<ISerializer<T>, ISerializer<T>> select) => _select = select;

		public ISerializer Get(ISerializer parameter) => _select(parameter.For<T>()).Adapt();
	}

	sealed class SerializerComposer : DelegatedAlteration<ISerializer>, ISerializerComposer
	{
		public SerializerComposer(Func<ISerializer, ISerializer> alteration) : base(alteration) {}
	}
}