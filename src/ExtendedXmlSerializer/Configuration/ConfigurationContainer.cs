// MIT License
//
// Copyright (c) 2016-2018 Wojciech Nagórski
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

using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Content.Registration;
using ExtendedXmlSerializer.ExtensionModel.Services;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Serializers = ExtendedXmlSerializer.ContentModel.Serializers;

namespace ExtendedXmlSerializer.Configuration
{
	public interface IConfigurationRoot : IActivator<ISerializers> {}

	public class ConfigurationRoot : ConfigurationContainer<ISerializers>, IConfigurationRoot
	{
		public ConfigurationRoot() : base(new ExtensionCollection(), XmlConfiguration.Default) {}
		public ConfigurationRoot(IExtensionCollection extensions) : base(extensions, XmlConfiguration.Default) {}
		public ConfigurationRoot(IActivator<ISerializers> activator) : base(activator) {}
	}

	public interface IConfigurationContainer : IActivator<IExtendedXmlSerializer> {}

	sealed class XmlConfiguration : Configuration
	{
		public static XmlConfiguration Default { get; } = new XmlConfiguration();
		XmlConfiguration() : base(Registrations.Default.Concat(Configurations.Default)) {}
	}

	public class ConfigurationContainer : ConfigurationContainer<IExtendedXmlSerializer>, IConfigurationContainer
	{
		public ConfigurationContainer() : base(new ExtensionCollection(), XmlConfiguration.Default) {}

		public ConfigurationContainer(params ISerializerExtension[] extensions)
			: base(ExtensionCollections.Default.Get(extensions.ToImmutableArray()),
			       XmlConfiguration.Default) {}

		public ConfigurationContainer(IExtensionCollection extensions) : base(extensions, XmlConfiguration.Default) {}
	}

	public class ConfigurationContainer<T> : ConfigurationElement, IActivator<T>
	{
		readonly IActivator<T> _activator;

		public ConfigurationContainer(IExtensionCollection extensions) : this(extensions, DefaultConfiguration.Default) {}

		public ConfigurationContainer(IExtensionCollection extensions, IConfiguration configuration)
			: this(Activators<T>.Default.Get(extensions), configuration) {}

		public ConfigurationContainer(IActivator<T> activator, IConfiguration configuration)
			: this(configuration.Executed(activator).Return(activator)) {}

		public ConfigurationContainer(IActivator<T> activator) : base(activator.Extensions) => _activator = activator;

		public T Get() => _activator.Get();
	}

	public interface IConfiguration : ICommand<IConfigurationElement> {}

	class PropertyAssignment<T, TProperty> : PropertyAssignment<TProperty>
	{
		public PropertyAssignment(IProperty<TProperty> property, TProperty value)
			: base(Types<T>.Default.Out(SourceCoercer<IMetadata>.Default).Get, property, value) {}
	}

	public static class Register
	{
		public static IConfiguration For<T>(Func<string, T> read, Func<T, string> write) => For(Serializers.New(read, write));

		public static IConfiguration For<T>(IContentSerializer<T> instance) => new RegisterSerializer<T>(instance);
	}

	class RegisterSerializer<T> : ServicePropertyAssignment<T, IContentSerializer<T>>
	{
		public RegisterSerializer(Func<string, T> read, Func<T, string> write)
			: this(new ContentModel.Content.ContentReader<T>(read), new ContentModel.Content.ContentWriter<T>(write)) {}

		public RegisterSerializer(IContentReader<T> read, IContentWriter<T> write)
			: this(new ContentSerializer<T>(read, write)) {}

		public RegisterSerializer(IContentSerializer<T> instance)
			: base(RegisteredSerializersProperty<T>.Default, instance) {}
	}

	sealed class DefaultConfiguration : Configuration
	{
		public static Configuration Default { get; } = new DefaultConfiguration();
		DefaultConfiguration() : base(Configurations.Default) {}
	}

	class Configuration : CompositeCommand<IConfigurationElement>, IConfiguration
	{
		public Configuration(IEnumerable<IConfiguration> items) : base(items.Cast<ICommand<IConfigurationElement>>()
		                                                                       .ToArray()) {}
	}

	class Configurations : Items<IConfiguration>
	{
		public static Configurations Default { get; } = new Configurations();
		Configurations() : this(Register.For(ReflectionSerializer<TypeInfo>.Default),
		                        Register.For(ReflectionSerializer<Type>.Default)) {}

		public Configurations(params IConfiguration[] items) : base(items) {}
	}

	class ServicePropertyAssignment<T, TProperty, TType> : ServicePropertyAssignment<T, IService<TProperty>> where TType : TProperty
	{
		public ServicePropertyAssignment(IProperty<IService<IService<TProperty>>> property)
			: base(property, A<TType>.Default.Get()) {}
	}

	class ServicePropertyAssignment<T, TProperty> : PropertyAssignment<T, IService<TProperty>>
	{
		public ServicePropertyAssignment(IProperty<IService<TProperty>> property, TProperty instance)
			: this(property, new InstanceService<TProperty>(instance)) {}

		public ServicePropertyAssignment(IProperty<IService<TProperty>> property, Type serviceType)
			: this(property, new Service<TProperty>(serviceType)) {}

		public ServicePropertyAssignment(IProperty<IService<TProperty>> property, IService<TProperty> value) : base(property, value) {}
	}

	sealed class Types<T> : Types
	{
		public static Types<T> Default { get; } = new Types<T>();
		Types() : base(Support<T>.Key) {}
	}

	class Types : IParameterizedSource<IConfigurationElement, ITypeConfiguration>
	{
		readonly TypeInfo _type;

		public Types(TypeInfo type) => _type = type;

		public ITypeConfiguration Get(IConfigurationElement parameter) => parameter.GetTypeConfiguration(_type);
	}

	class PropertyAssignment<T> : IConfiguration
	{
		readonly Func<IConfigurationElement, IMetadata> _metadata;
		readonly IProperty<T> _property;
		readonly T _value;

		public PropertyAssignment(Func<IConfigurationElement, IMetadata> metadata, IProperty<T> property, T value)
		{
			_metadata = metadata;
			_property = property;
			_value = value;
		}

		public void Execute(IConfigurationElement parameter)
		{
			_property.Assign(_metadata(parameter), _value);
		}
	}
}