using System;
using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Content {
	sealed class GenericElement : IElements
	{
		readonly IIdentities _identities;
		readonly IGeneric<IIdentity, ImmutableArray<Type>, IWriter> _adapter;

		public GenericElement(IIdentities identities) : this(identities, Adapter.Default) {}

		public GenericElement(IIdentities identities, IGeneric<IIdentity, ImmutableArray<Type>, IWriter> adapter)
		{
			_identities = identities;
			_adapter = adapter;
		}

		public IWriter Get(TypeInfo parameter)
			=> _adapter.Get(parameter)
			           .Invoke(_identities.Get(parameter), parameter.GetGenericArguments()
			                                                        .ToImmutableArray());

		sealed class Adapter : Generic<IIdentity, ImmutableArray<Type>, IWriter>
		{
			public static Adapter Default { get; } = new Adapter();
			Adapter() : base(typeof(Writer<>)) {}

			sealed class Writer<T> : GenericWriterAdapter<T>
			{
				public Writer(IIdentity identity, ImmutableArray<Type> arguments)
					: base(new GenericIdentity<T>(identity, arguments)) {}
			}
		}
	}
}