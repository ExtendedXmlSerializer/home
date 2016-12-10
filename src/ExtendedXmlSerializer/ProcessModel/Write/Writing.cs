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
using ExtendedXmlSerialization.Elements;
using ExtendedXmlSerialization.Services;

namespace ExtendedXmlSerialization.ProcessModel.Write
{
    public class Writing : IWriting
    {
        private readonly IWriter _writer;
        private readonly IAttachedProperties _properties;
        private readonly INamespaceLocator _locator;
        private readonly IWritingContext _context;
        private readonly IServiceProvider _services;

        public Writing(IWriter writer, IWritingContext context, INamespaceLocator locator, params object[] services)
            : this(writer, context, AttachedProperties.Default, locator, new CompositeServiceProvider(services)) {}

        public Writing(IWriter writer, IWritingContext context, IAttachedProperties properties,
                       INamespaceLocator locator, IServiceProvider services)
        {
            _writer = writer;
            _context = context;
            _properties = properties;
            _locator = locator;
            _services = services;
        }

        public object GetService(Type serviceType)
            => serviceType.GetTypeInfo().IsInstanceOfType(this) ? this : _services.GetService(serviceType);

        public void Start(IRootElement root) => _writer.Start(root);
        public void Begin(IElement element) => _writer.Begin(element);
        public void EndElement() => _writer.EndElement();
        public void Emit(object instance) => _writer.Emit(instance);
        public void Emit(IProperty property) => _writer.Emit(property);

        public void Dispose() => _writer.Dispose();

        public void Attach(IProperty property) => _properties.Attach(_context.Current.Instance, property);

        public IImmutableList<IProperty> GetProperties()
        {
            var list = _properties.GetProperties(_context.Current.Instance);
            var result = list.ToArray().ToImmutableList();
            list.Clear();
            return result;
        }

        public IDisposable Start(object root) => _context.Start(root);
        public IDisposable New(object instance) => _context.New(instance);
        public IDisposable New(IImmutableList<MemberInfo> members) => _context.New(members);
        public IDisposable New(MemberInfo member) => _context.New(member);
        public IDisposable ToMemberContext() => _context.ToMemberContext();

        public WriteContext Current => _context.Current;
        public IEnumerable<WriteContext> Hierarchy => _context.Hierarchy;
        public Uri Get(object parameter) => _locator.Get(parameter);
    }
}