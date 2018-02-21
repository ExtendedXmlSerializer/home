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

using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Collections;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Services;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IServiceProvider = System.IServiceProvider;

namespace ExtendedXmlSerializer.Configuration
{
	sealed class ConfigurationServicesExtension : ISerializerExtension, IServiceProvider, ICommand<object>
	{
		readonly IConfigurationElement _element;
		readonly IConfiguredTypes _types;
		readonly ICollection<object> _services;
		readonly IServiceProvider _provider;

		public ConfigurationServicesExtension(IConfigurationElement element, IConfiguredTypes types, params object[] services)
			: this(element, types, services.ToList()) {}

		public ConfigurationServicesExtension(IConfigurationElement element, IConfiguredTypes types, IList<object> services)
			: this(element, types, services, new ServiceProvider(services)) {}

		public ConfigurationServicesExtension(IConfigurationElement element, IConfiguredTypes types, ICollection<object> services, IServiceProvider provider)
		{
			_element = element;
			_types = types;
			_services = services;
			_provider = provider;
		}

		IServiceRepository IParameterizedSource<IServiceRepository, IServiceRepository>.Get(IServiceRepository parameter)
			=> parameter.RegisterInstance(_element)
			            .RegisterInstance<IMetadataTable>(new MetadataTable(_types.SelectMany(x => x.Prepending(x.Get()))));

		void ICommand<IServices>.Execute(IServices parameter) {}

		public object GetService(System.Type serviceType) => _provider.GetService(serviceType);
		public void Execute(object parameter) => _services.Add(parameter);
	}

	class PropertyValue<TProperty, T> : FixedParameterSource<IConfigurationElement, T> where TProperty : class, IProperty<T>
	{
		public PropertyValue(ISingletonLocator locator, IConfigurationElement parameter)
			: this(locator.Out(A<TProperty>.Default).Get(typeof(TProperty)), parameter) {}
		public PropertyValue(IParameterizedSource<IConfigurationElement, T> source, IConfigurationElement parameter) : base(source, parameter) {}
	}

	class MetadataValue<TProperty, T> : SpecificationSource<MemberInfo, T> where TProperty : class, IMetadataProperty<T>
	{
		public MetadataValue(IMetadataTable table, ISingletonLocator locator)
			: this(table, locator.Out(A<TProperty>.Default).Get(typeof(TProperty)).Assigned()) {}

		public MetadataValue(IMetadataTable table, ISpecificationSource<IMetadata, T> property)
			: base(property.To(table), property.In(table)) {}
	}

	public interface IMetadataTable : IValueSource<MemberInfo, IMetadata> {}

	sealed class MetadataTable : TableValueSource<MemberInfo, IMetadata>, IMetadataTable
	{
		public MetadataTable(IEnumerable<IMetadata> metadata) : base(metadata.ToDictionary(x => x.Get()))  {}
	}
}