using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core;
using System;
using System.Reflection;
using System.Runtime.Serialization;

#if NET45 // http://www.nooooooooooooooo.com/ ... Hashtags belong on Twitter. ;)
namespace System.Runtime.Serialization
{
/// <exclude />
	public interface ISerializationSurrogateProvider
	{
		object GetDeserializedObject(object obj, Type t);
		object GetObjectToSerialize(Object obj, Type t);
		Type GetSurrogateType(Type t);
	}
}
#endif

namespace ExtendedXmlSerializer.ExtensionModel
{
	sealed class SurrogatesExtension : ISerializerExtension
	{
		readonly ISerializationSurrogateProvider _provider;

		public SurrogatesExtension(ISerializationSurrogateProvider provider) => _provider = provider;

		public IServiceRepository Get(IServiceRepository parameter) => parameter.RegisterInstance(_provider)
		                                                                        .Decorate<IContents, Contents>();

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class Contents : IContents
		{
			readonly ISerializationSurrogateProvider _provider;
			readonly IContents                       _contents;

			public Contents(ISerializationSurrogateProvider provider, IContents contents)
			{
				_provider = provider;
				_contents = contents;
			}

			public ContentModel.ISerializer Get(TypeInfo parameter)
			{
				var surrogateType = _provider.GetSurrogateType(parameter);
				var result = surrogateType != null
					             ? new Serializer(_provider, new Mapping(parameter.AsType(), surrogateType),
					                              _contents.Get(surrogateType))
					             : _contents.Get(parameter);
				return result;
			}

			readonly struct Mapping
			{
				public Mapping(Type origin, Type surrogate)
				{
					Origin    = origin;
					Surrogate = surrogate;
				}

				public Type Origin { get; }

				public Type Surrogate { get; }
			}

			sealed class Serializer : ContentModel.ISerializer
			{
				readonly ISerializationSurrogateProvider _provider;
				readonly Mapping                         _mapping;
				readonly ContentModel.ISerializer                     _serializer;

				public Serializer(ISerializationSurrogateProvider provider, Mapping mapping, ContentModel.ISerializer serializer)
				{
					_provider   = provider;
					_mapping    = mapping;
					_serializer = serializer;
				}

				public object Get(IFormatReader parameter)
				{
					var surrogate = _serializer.Get(parameter);
					var result    = _provider.GetDeserializedObject(surrogate, _mapping.Origin);
					return result;
				}

				public void Write(IFormatWriter writer, object instance)
				{
					var surrogate = _provider.GetObjectToSerialize(instance, _mapping.Surrogate);
					_serializer.Write(writer, surrogate);
				}
			}
		}
	}
}