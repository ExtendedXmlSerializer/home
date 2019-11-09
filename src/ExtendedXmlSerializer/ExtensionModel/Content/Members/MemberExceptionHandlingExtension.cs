using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using System;
using System.Xml;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class MemberExceptionHandlingExtension : ISerializerExtension
	{
		public static MemberExceptionHandlingExtension Default { get; } = new MemberExceptionHandlingExtension();

		MemberExceptionHandlingExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate<IMemberSerializers, MemberSerializers>()
			            .Decorate<IMemberHandler, MemberHandler>();

		public void Execute(IServices parameter) {}

		sealed class MemberHandler : IMemberHandler
		{
			readonly IMemberHandler _handler;

			public MemberHandler(IMemberHandler handler) => _handler = handler;

			public void Handle(IInnerContent contents, IMemberSerializer member)
			{
				try
				{
					_handler.Handle(contents, member);
				}
				catch (Exception e)
				{
					var line = contents.Get()
					                   .Get()
					                   .To<IXmlLineInfo>();
					// ReSharper disable once ThrowFromCatchWithNoInnerException
					// ReSharper disable once ThrowFromCatchWithNoInnerException
					throw new
						InvalidOperationException($"An exception was encountered while deserializing member '{member.Profile.Metadata.ReflectedType}.{member.Profile.Name}'.",
						                          new XmlException(e.Message, e, line.LineNumber, line.LinePosition));
				}
			}
		}

		sealed class MemberSerializers : IMemberSerializers
		{
			readonly IMemberSerializers _serializers;

			public MemberSerializers(IMemberSerializers serializers) => _serializers = serializers;

			public IMemberSerializer Get(IMember parameter)
			{
				var origin     = _serializers.Get(parameter);
				var serializer = new MemberSerializer(origin);
				var result = origin is PropertyMemberSerializer property
					             ? new PropertyMemberSerializer(property)
					             : origin is IRuntimeSerializer runtime
						             ? (IMemberSerializer)new RuntimeSerializer(runtime)
						             : serializer;
				return result;
			}
		}

		sealed class RuntimeSerializer : IRuntimeSerializer
		{
			readonly IRuntimeSerializer _serializer;

			public RuntimeSerializer(IRuntimeSerializer serializer) => _serializer = serializer;

			public object Get(IFormatReader parameter) => _serializer.Get(parameter);

			public void Write(IFormatWriter writer, object instance)
			{
				_serializer.Write(writer, instance);
			}

			public IMember Profile => _serializer.Profile;

			public IMemberAccess Access => _serializer.Access;

			public IMemberSerializer Get(object parameter) => _serializer.Get(parameter);
		}

		sealed class MemberSerializer : IMemberSerializer
		{
			readonly IMemberSerializer _serializer;

			public MemberSerializer(IMemberSerializer serializer) => _serializer = serializer;

			public object Get(IFormatReader parameter) => _serializer.Get(parameter);

			public void Write(IFormatWriter writer, object instance)
			{
				try
				{
					_serializer.Write(writer, instance);
				}
				catch (Exception e)
				{
					throw new
						InvalidOperationException($"An exception was encountered while serializing member '{Profile.Metadata.ReflectedType}.{Profile.Name}'.  Provided instance is '{instance}'.",
						                          e);
				}
			}

			public IMember Profile => _serializer.Profile;

			public IMemberAccess Access => _serializer.Access;
		}
	}
}