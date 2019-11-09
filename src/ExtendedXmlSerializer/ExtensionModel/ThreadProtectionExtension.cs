using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel
{
	sealed class ThreadProtectionExtension : ISerializerExtension
	{
		public static ThreadProtectionExtension Default { get; } = new ThreadProtectionExtension();

		ThreadProtectionExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate(typeof(ISerializer<,>), typeof(Serializer<,>));

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class Serializer<TRead, TWrite> : ISerializer<TRead, TWrite>
		{
			readonly ISerializer<TRead, TWrite> _serializer;

			public Serializer(ISerializer<TRead, TWrite> serializer) => _serializer = serializer;

			public object Deserialize(TRead reader)
			{
				lock (_serializer)
				{
					return _serializer.Deserialize(reader);
				}
			}

			public void Serialize(TWrite writer, object instance)
			{
				lock (_serializer)
				{
					_serializer.Serialize(writer, instance);
				}
			}
		}
	}
}