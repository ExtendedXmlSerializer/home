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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Cache;

namespace ExtendedXmlSerialization.Services
{
    public class CompositeServiceProvider : WeakCacheBase<Type, object>, IServiceProvider
    {
        private readonly IEnumerable<IServiceProvider> _providers;
        private readonly IEnumerable<object> _services;

        public CompositeServiceProvider(params object[] services)
            : this(services.OfType<IServiceProvider>().ToImmutableList(), services) {}

        public CompositeServiceProvider(IEnumerable<IServiceProvider> providers, params object[] services)
            : this(providers, services.AsEnumerable()) {}

        public CompositeServiceProvider(IEnumerable<object> services)
            : this(Enumerable.Empty<IServiceProvider>(), services) {}

        public CompositeServiceProvider(IEnumerable<IServiceProvider> providers, IEnumerable<object> services)
        {
            _providers = providers;
            _services = services;
        }

        public object GetService(Type serviceType) => Get(serviceType);

        protected override object Callback(Type key)
            => _services.FirstOrDefault(key.GetTypeInfo().IsInstanceOfType) ?? FromServices(key);

        private object FromServices(Type serviceType)
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
    }
}