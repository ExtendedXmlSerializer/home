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
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerialization.Core
{
    public class CompositeServiceProvider : WeakCacheBase<Type, object>, IDisposable, IServiceProvider
    {
        private readonly IImmutableList<IServiceProvider> _providers;
        private readonly IImmutableList<object> _services;

        public CompositeServiceProvider(params object[] services)
            : this(services.OfType<IServiceProvider>().ToImmutableList(), services.ToImmutableList()) {}

        /*public CompositeServiceProvider(IEnumerable<IServiceProvider> providers, params object[] services)
            : this(providers.ToImmutableList(), services.ToImmutableList()) {}*/

        /*public CompositeServiceProvider(IEnumerable<object> services)
            : this(Enumerable.Empty<IServiceProvider>(), services) {}*/

        public CompositeServiceProvider(IImmutableList<IServiceProvider> providers, IImmutableList<object> services)
        {
            _providers = providers;
            _services = services;
        }

        public object GetService(Type serviceType) => Get(serviceType);

        protected override object Create(Type parameter) => FromServices(parameter) ?? FromProviders(parameter);

        private object FromServices(Type key)
        {
            var typeInfo = key.GetTypeInfo();
            var count = _services.Count;
            for (var i = 0; i < count; i++)
            {
                var service = _services[i];
                if (typeInfo.IsInstanceOfType(service))
                {
                    return service;
                }
            }

            return null;
        }

        private object FromProviders(Type serviceType)
        {
            foreach (var service in _providers)
            {
                var result = service.GetService(serviceType);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        ~CompositeServiceProvider()
        {
            OnDispose(false);
        }

        public void Dispose()
        {
            OnDispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void OnDispose(bool disposing)
        {
            var count = _services.Count;
            for (var i = 0; i < count; i++)
            {
                var disposable = _services[i] as IDisposable;
                disposable?.Dispose();
            }
        }
    }
}