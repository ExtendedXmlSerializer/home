// MIT License
//
// Copyright (c) 2016 Wojciech Nagórski
//                    Michael DeMond
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Concurrent;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ReflectionModel;
using IContents = ExtendedXmlSerializer.ContentModel.Content.IContents;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	class Activated<T> : Generic<object, T>, IParameterizedSource<IServices, T>, IAlteration<IServiceRepository> where T : class
	{
		readonly ISingletonLocator _locator;
		readonly Type _objectType;
		readonly TypeInfo _targetType;

		public Activated(Type objectType, TypeInfo targetType, Type definition) : this(SingletonLocator.Default, objectType, targetType, definition) {}

		public Activated(ISingletonLocator locator, Type objectType, TypeInfo targetType, Type definition) : base(definition)
		{
			_locator = locator;
			_objectType = objectType;
			_targetType = targetType;
		}

		public T Get(IServices parameter)
		{
			var service = parameter.GetService(_objectType);

			var result = service as T ?? Get(_targetType.Yield().ToImmutableArray()).Invoke(service);
			return result;
		}

		public IServiceRepository Get(IServiceRepository parameter)
		{
			var singleton = _locator.Get(_objectType);
			var result = singleton != null ? parameter.RegisterInstance(_objectType, singleton) : parameter.Register(_objectType);
			return result;
		}
	}

	sealed class ActivatedXmlSerializer : Activated<IExtendedXmlCustomSerializer>, IExtendedXmlCustomSerializer
	{
		public ActivatedXmlSerializer(Type objectType, TypeInfo targetType) : base(objectType, targetType, typeof(Adapter<>)) {}

		public object Deserialize(XElement xElement) => Throw();

		public void Serializer(System.Xml.XmlWriter xmlWriter, object instance)
		{
			Throw();
		}

		static object Throw() =>
			throw new NotSupportedException("This serializer is used as a marker to activate another serializer at runtime.");
	}

	sealed class ActivatedSerializer : Activated<ISerializer>, ISerializer
	{
		public ActivatedSerializer(Type objectType, TypeInfo targetType) : base(objectType, targetType, typeof(GenericSerializerAdapter<>)) {}

		static object Throw() =>
			throw new NotSupportedException("This serializer is used as a marker to activate another serializer at runtime.");

		public object Get(IFormatReader parameter) => Throw();

		public void Write(IFormatWriter writer, object instance)
		{
			Throw();
		}
	}

	class Metadata<TMember, T> : TableSource<TMember, T>, ISerializerExtension where TMember : MemberInfo
	{
		readonly IDictionary<TMember, T> _store;

		public Metadata(IEqualityComparer<TMember> comparer) : this(new ConcurrentDictionary<TMember, T>(comparer)) {}

		public Metadata(IDictionary<TMember, T> store) : base(store) => _store = store;


		public IServiceRepository Get(IServiceRepository parameter) => _store.Values.OfType<IAlteration<IServiceRepository>>()
		                                                                     .Aggregate(parameter,
		                                                                                (repository, serializer) =>
			                                                                                serializer.Get(repository));

		public void Execute(IServices parameter)
		{
			foreach (var pair in _store.ToArray())
			{
				if (pair.Value is IParameterizedSource<IServices, T> serializer)
				{
					_store[pair.Key] = serializer.Get(parameter);
				}
			}
		}
	}

	public interface ICustomXmlSerializers : ITypedTable<IExtendedXmlCustomSerializer>, ISerializerExtension {}
	sealed class CustomXmlSerializers : Metadata<TypeInfo, IExtendedXmlCustomSerializer>, ICustomXmlSerializers
	{
		public CustomXmlSerializers() : base(ReflectionModel.Defaults.TypeComparer) {}
	}

	public interface ICustomSerializers : ITypedTable<ISerializer>, ISerializerExtension {}
	sealed class CustomSerializers : Metadata<TypeInfo, ISerializer>, ICustomSerializers
	{
		public CustomSerializers() : base(ReflectionModel.Defaults.TypeComparer) {}
	}

	public interface ICustomMemberSerializers : IMemberTable<ISerializer>, ISerializerExtension {}
	sealed class MemberCustomSerializers : Metadata<MemberInfo, ISerializer>, ICustomMemberSerializers
	{
		public MemberCustomSerializers() : base(MemberComparer.Default) {}
	}


	sealed class CustomSerializationExtension : ISerializerExtension
	{
		public CustomSerializationExtension() : this(new CustomXmlSerializers(), new CustomSerializers(), new MemberCustomSerializers()) {}

		public CustomSerializationExtension(ICustomXmlSerializers xmlSerializers, ICustomSerializers types, ICustomMemberSerializers members)
		{
			XmlSerializers = xmlSerializers;
			Types = types;
			Members = members;
		}

		public ICustomXmlSerializers XmlSerializers { get; }

		public ICustomSerializers Types { get; }

		public ICustomMemberSerializers Members { get; }

		public IServiceRepository Get(IServiceRepository parameter) => Extensions().Aggregate(parameter,
		                                                                          (repository, serializer) =>
			                                                                          serializer.Get(repository))
		                                                               .RegisterInstance(XmlSerializers)
		                                                               .RegisterInstance(Types)
		                                                               .RegisterInstance(Members)
		                                                               .Register<RegisteredMemberContents>()
		                                                               .Decorate<IContents, Contents>();

		void ICommand<IServices>.Execute(IServices parameter)
		{
			foreach (var extension in Extensions())
			{
				extension.Execute(parameter);
			}
		}

		IEnumerable<ISerializerExtension> Extensions() => new ISerializerExtension[] {XmlSerializers, Types, Members};

		sealed class Contents : IContents
		{
			readonly ICustomXmlSerializers _custom;
			readonly IContents _contents;

			public Contents(ICustomXmlSerializers custom, IContents contents)
			{
				_custom = custom;
				_contents = contents;
			}

			public ISerializer Get(TypeInfo parameter)
			{
				var custom = _custom.Get(parameter);
				var result = custom != null ? new Serializer(custom) : _contents.Get(parameter);
				return result;
			}

			sealed class Serializer : ISerializer
			{
				readonly IExtendedXmlCustomSerializer _custom;

				public Serializer(IExtendedXmlCustomSerializer custom) => _custom = custom;

				public object Get(IFormatReader parameter)
				{
					var reader = parameter.Get().AsValid<System.Xml.XmlReader>();
					var subtree = reader.ReadSubtree();
					var element = XElement.Load(subtree);
					var result = _custom.Deserialize(element);
					return result;
				}

				public void Write(IFormatWriter writer, object instance)
					=> _custom.Serializer(writer.Get().AsValid<System.Xml.XmlWriter>(), instance);
			}
		}
	}
}