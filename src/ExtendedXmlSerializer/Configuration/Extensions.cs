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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.Configuration
{
	/*sealed class TypeConfigurationExtension : ISerializerExtension, IValueSource<TypeInfo, ITypeConfiguration>
	{
		public IServiceRepository Get(IServiceRepository parameter) => parameter;

		void ICommand<IServices>.Execute(IServices parameter) {}
		public ITypeConfiguration Get(TypeInfo parameter)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<ITypeConfiguration> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public ImmutableArray<ITypeConfiguration> Get()
		{
			throw new NotImplementedException();
		}
	}*/

	/*
		sealed class Service<T> : ISource<T>
		{
			readonly IEnumerable<ISerializerExtension> _extensions;
			readonly IServicesFactory _factory;

			public Service(IEnumerable<ISerializerExtension> extensions) : this(extensions, ServicesFactory.Default) {}

			public Service(IEnumerable<ISerializerExtension> extensions, IServicesFactory factory)
			{
				_extensions = extensions;
				_factory = factory;
			}

			public T Get()
			{
				using (var services = _services())
				{
					var result = services.Get<T>();
					return result;
				}
			}
		}
	*/

	/*sealed class ServiceExtension<T> : DecoratedSource<T>, ISerializerExtension
	{
		public ServiceExtension(IExtensionCollection extensions) : this(new Service<T>(extensions)) { }

		public ServiceExtension(ISource<T> source) : base(source) {}

		IServiceRepository IParameterizedSource<IServiceRepository, IServiceRepository>.Get(IServiceRepository parameter) => parameter;

		void ICommand<IServices>.Execute(IServices parameter) { }
	}*/

	sealed class Extensions : Enumerable<ISerializerExtension>, IExtensions
	{
		readonly ICollection<ISerializerExtension> _extensions;

		public Extensions(ICollection<ISerializerExtension> extensions) : base(extensions) => _extensions = extensions;


		public void Execute(ISerializerExtension parameter)
		{
			_extensions.AddOrReplace(parameter);
		}

		public void Execute(TypeInfo parameter)
		{
			var extension = _extensions.FirstOrDefault(parameter.IsInstanceOfType);
			if (extension != null)
			{
				_extensions.Remove(extension);
			}
		}
	}
}