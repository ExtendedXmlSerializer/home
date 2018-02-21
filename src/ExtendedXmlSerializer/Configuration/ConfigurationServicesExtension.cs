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
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IServiceProvider = System.IServiceProvider;

namespace ExtendedXmlSerializer.Configuration
{
	sealed class ConfigurationServicesExtension : ISerializerExtension, IServiceProvider, ICommand<object>
	{
		readonly ICollection<object> _services;
		readonly IServiceProvider _provider;

		[UsedImplicitly]
		public ConfigurationServicesExtension() : this(Support<object>.Empty) {}

		public ConfigurationServicesExtension(params object[] services) : this(services.ToList()) {}

		public ConfigurationServicesExtension(IList<object> services) : this(services, new ServiceProvider(services)) {}

		public ConfigurationServicesExtension(ICollection<object> services, IServiceProvider provider)
		{
			_services = services;
			_provider = provider;
		}

		IServiceRepository IParameterizedSource<IServiceRepository, IServiceRepository>.Get(IServiceRepository parameter)
			=> parameter.RegisterInstance<IMetadataTable>(new MetadataTable(_provider.GetValid<IConfiguredTypes>()
			                                                                         .SelectMany(x => x.Prepending(x.Get()))));

		void ICommand<IServices>.Execute(IServices parameter) {}

		public object GetService(System.Type serviceType) => _provider.GetService(serviceType);
		public void Execute(object parameter) => _services.Add(parameter);
	}

	class PropertyReference<TProperty, T> : FixedParameterSource<IConfigurationElement, T> where TProperty : class, IProperty<T>
	{
		public PropertyReference(ISingletonLocator locator, IConfigurationElement parameter)
			: this(locator.Out(A<TProperty>.Default).Get(typeof(TProperty)), parameter) {}
		public PropertyReference(IParameterizedSource<IConfigurationElement, T> source, IConfigurationElement parameter) : base(source, parameter) {}
	}

	class MetadataPropertyReference<TProperty, T> : SpecificationSource<MemberInfo, T> where TProperty : class, IMetadataProperty<T>
	{
		public MetadataPropertyReference(IMetadataTable table, ISingletonLocator locator)
			: this(table, locator.Out(A<TProperty>.Default).Get(typeof(TProperty)).Assigned()) {}

		public MetadataPropertyReference(IMetadataTable table, ISpecificationSource<IMetadata, T> property)
			: base(property.To(table), property.In(table)) {}
	}

	public interface IMetadataTable : IValueSource<MemberInfo, IMetadata> {}

	sealed class MetadataTable : TableValueSource<MemberInfo, IMetadata>, IMetadataTable
	{
		public MetadataTable(IEnumerable<IMetadata> metadata) : base(metadata.ToDictionary(x => x.Get()))  {}
	}
}