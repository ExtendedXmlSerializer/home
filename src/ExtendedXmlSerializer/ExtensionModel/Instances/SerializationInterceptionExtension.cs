using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Instances
{
	sealed class SerializationInterceptionExtension
		: ISerializerExtension, IAssignable<TypeInfo, ISerializationInterceptor>
	{
		readonly ITypedTable<ISerializationInterceptor> _registrations;

		[UsedImplicitly]
		public SerializationInterceptionExtension() : this(new TypedTable<ISerializationInterceptor>()) {}

		public SerializationInterceptionExtension(ITypedTable<ISerializationInterceptor> registrations)
			=> _registrations = registrations;

		public IServiceRepository Get(IServiceRepository parameter) => parameter.Decorate<IActivators>(Register)
		                                                                        .Decorate<IContents>(Register);

		IContents Register(IServiceProvider _, IContents previous) => new Contents(_registrations, previous);

		IActivators Register(IServiceProvider _, IActivators previous) => new Activators(_registrations, previous);

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class Contents : IContents
		{
			readonly ITypedTable<ISerializationInterceptor> _interceptors;
			readonly IContents                              _contents;

			public Contents(ITypedTable<ISerializationInterceptor> interceptors, IContents contents)
			{
				_interceptors = interceptors;
				_contents     = contents;
			}

			public ISerializer Get(TypeInfo parameter)
			{
				var previous = _contents.Get(parameter);
				var result   = _interceptors.IsSatisfiedBy(parameter) ? Create(parameter, previous) : previous;
				return result;
			}

			Serializer Create(TypeInfo parameter, ISerializer previous)
			{
				var interceptor = _interceptors.Get(parameter);
				var result      = new Serializer(new Reader(interceptor, previous), new Writer(interceptor, previous));
				return result;
			}
		}

		sealed class Activators : IActivators
		{
			readonly ITypedTable<ISerializationInterceptor> _table;
			readonly IActivators                            _activators;

			public Activators(ITypedTable<ISerializationInterceptor> table, IActivators activators)
			{
				_table      = table;
				_activators = activators;
			}

			public IActivator Get(Type parameter)
				=> new Activator(parameter, _activators.Get(parameter), _table.Get(parameter));

			sealed class Activator : IActivator
			{
				readonly Type                      _instanceType;
				readonly IActivator                _activator;
				readonly ISerializationInterceptor _interceptor;

				public Activator(Type instanceType, IActivator activator, ISerializationInterceptor interceptor)
				{
					_instanceType = instanceType;
					_activator    = activator;
					_interceptor  = interceptor;
				}

				public object Get() => _interceptor.Activating(_instanceType) ?? _activator.Get();
			}
		}

		sealed class Writer : IWriter
		{
			readonly ISerializationInterceptor _interceptor;
			readonly IWriter                   _writer;

			public Writer(ISerializationInterceptor interceptor, IWriter writer)
			{
				_interceptor = interceptor;
				_writer      = writer;
			}

			public void Write(IFormatWriter writer, object instance)
			{
				_writer.Write(writer, _interceptor.Serializing(writer, instance));
			}
		}

		sealed class Reader : IReader
		{
			readonly ISerializationInterceptor _interceptor;
			readonly IReader                   _reader;

			public Reader(ISerializationInterceptor interceptor, IReader reader)
			{
				_interceptor = interceptor;
				_reader      = reader;
			}

			public object Get(IFormatReader parameter)
			{
				var instance = _reader.Get(parameter);
				var result   = _interceptor.Deserialized(parameter, instance);
				return result;
			}
		}

		public void Assign(TypeInfo key, ISerializationInterceptor value)
		{
			_registrations.Assign(key, value);
		}
	}
}