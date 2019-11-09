using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class Element : IElement
	{
		readonly IIdentities                  _identities;
		readonly IGeneric<IIdentity, IWriter> _adapter;

		public Element(IIdentities identities) : this(identities, Adapter.Default) {}

		public Element(IIdentities identities, IGeneric<IIdentity, IWriter> adapter)
		{
			_identities = identities;
			_adapter    = adapter;
		}

		public IWriter Get(TypeInfo parameter) => _adapter.Get(parameter)
		                                                  .Invoke(_identities.Get(parameter));

		sealed class Adapter : Generic<IIdentity, IWriter>
		{
			public static Adapter Default { get; } = new Adapter();

			Adapter() : base(typeof(Writer<>)) {}

			sealed class Writer<T> : GenericWriterAdapter<T>
			{
				public Writer(IIdentity identity) : base(new Identity<T>(identity)) {}
			}
		}
	}
}