using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel
{
	/// <summary>
	/// Configures a serializer so that if an exception occcurs during selection of a serializer for a given type, a
	/// provided fallback serializer is selected instead.  By default the provided fallback serializer does nothing for
	/// serializer and returns a null instance for deserialization.
	/// </summary>
	/// <seealso href="https://github.com/ExtendedXmlSerializer/ExtendedXmlSerializer/issues/197"/>
	public class FallbackSerializationExtension : ISerializerExtension
	{
		/// <summary>
		/// The default instance.  By default the provided fallback serializer does nothing for
		/// serializer and returns a null instance for deserialization.
		/// </summary>
		public static FallbackSerializationExtension Default { get; } = new FallbackSerializationExtension();

		FallbackSerializationExtension() : this(EmptySerializer.Instance) {}

		readonly IFallbackSerializer _fallback;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="fallback">The fallback serializer to use in case an exception is thrown during serialzer selection..
		/// </param>
		public FallbackSerializationExtension(IFallbackSerializer fallback) => _fallback = fallback;

		/// <inheritdoc />
		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterInstance(_fallback)
			            .Register<RuntimeSerialization>()
			            .Decorate<IRuntimeSerialization, RuntimeSerialization>();

		void ICommand<IServices>.Execute(IServices parameter) {}

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

		/// <summary>
		/// Used to register a specific serializer with the container so that it is used for fall back serialization.
		/// </summary>
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