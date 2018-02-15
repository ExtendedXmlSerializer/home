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
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Services;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
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

		IServiceRepository IParameterizedSource<IServiceRepository, IServiceRepository>.Get(IServiceRepository parameter) =>
			parameter;

		void ICommand<IServices>.Execute(IServices parameter) {}

		public object GetService(Type serviceType) => _provider.GetService(serviceType);
		public void Execute(object parameter) => _services.Add(parameter);
	}
}