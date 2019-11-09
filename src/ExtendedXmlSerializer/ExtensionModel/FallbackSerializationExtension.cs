using System;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ExtensionModel
{
	public class FallbackSerializationExtension : ISerializerExtension
	{
		public static FallbackSerializationExtension Default { get; } = new FallbackSerializationExtension();

		FallbackSerializationExtension() : this(EmptySerializer.Instance) {}

		readonly IFallbackSerializer _fallback;

		public FallbackSerializationExtension(IFallbackSerializer fallback) => _fallback = fallback;

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterInstance(_fallback)
			            .Register<RuntimeSerialization>()
			            .Decorate<IRuntimeSerialization, RuntimeSerialization>();

		public void Execute(IServices parameter) {}

		sealed class RuntimeSerialization : IRuntimeSerialization
		{
			readonly IRuntimeSerialization _serializers;
			readonly ISerializer           _fallback;

			public RuntimeSerialization(IRuntimeSerialization serializers, IFallbackSerializer fallback)
			{
				_serializers = serializers;
				_fallback    = fallback;
			}

			public ISerializer Get(TypeInfo parameter)
			{
				if (parameter != null)
				{
					try
					{
						return _serializers.Get(parameter);
					}
					catch (InvalidOperationException) {}
				}

				return _fallback;
			}
		}

		public interface IFallbackSerializer : ISerializer {}

		sealed class EmptySerializer : IFallbackSerializer
		{
			public static EmptySerializer Instance { get; } = new EmptySerializer();

			EmptySerializer() {}

			public object Get(IFormatReader parameter) => null;

			public void Write(IFormatWriter writer, object instance) {}
		}
	}
}