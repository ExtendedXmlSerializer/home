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
using ExtendedXmlSerializer.Core.LightInject;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Types;
using System;
using System.Linq;
using IServiceProvider = System.IServiceProvider;

namespace ExtendedXmlSerializer.Configuration
{
	sealed class ServicesFactory : IServicesFactory
	{
		public static ServicesFactory Default { get; } = new ServicesFactory();
		ServicesFactory() : this(ConstructorSelector.Default, new ContainerOptions {EnablePropertyInjection = false}) {}

		readonly IConstructorSelector _selector;
		readonly ContainerOptions _options;

		public ServicesFactory(IConstructorSelector selector, ContainerOptions options)
		{
			_selector = selector;
			_options = options;
		}

		public IServices Get(IExtensionCollection parameter)
		{
			var result = new Services(new ServiceContainer(_options) {ConstructorSelector = _selector});

			var services = result.RegisterInstance(parameter)
			                     .RegisterInstance<IServiceProvider>(new Provider(result.GetService));

			var extensions = parameter.OrderBy(x => x, SortComparer<ISerializerExtension>.Default)
			                          .Fixed();
			extensions.Alter(services);

			foreach (var extension in extensions)
			{
				extension.Execute(result);
			}
			return result;
		}

		sealed class Provider : DelegatedSource<Type, object>, IServiceProvider
		{
			public Provider(Func<Type, object> source) : base(source) {}

			public object GetService(Type serviceType) => Get(serviceType);
		}
	}
}