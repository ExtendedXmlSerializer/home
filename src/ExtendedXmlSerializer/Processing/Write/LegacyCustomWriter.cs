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
using System.Xml;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Model.Write;

namespace ExtendedXmlSerialization.Processing.Write
{
    class LegacyCustomWriter : IWriter
    {
        private readonly static IDisposable Empty = new DelegatedDisposable(() => { });

        private readonly ISerializationToolsFactory _tools;
        private readonly IWriter _inner;
        private readonly XmlWriter _writer;
        readonly private Action _complete;

        public LegacyCustomWriter(ISerializationToolsFactory tools, IWriter inner, XmlWriter writer)
        {
            _tools = tools;
            _inner = inner;
            _writer = writer;
            _complete = Enable;
        }

        void Enable() => Enabled = true;

        private bool Enabled { get; set; } = true;

        public IDisposable New(IContext context)
        {
            if (Enabled)
            {
                var result = _inner.New(context);
                var configuration = _tools.GetConfiguration(context.Entity.Type);
                if (configuration?.IsCustomSerializer ?? false)
                {
                    Enabled = false;
                    return new Context(configuration, _writer, context.Instance(), result, _complete);
                }
                return result;
            }
            return Empty;
        }

        public void Emit(IContext context)
        {
            if (Enabled || context is IProperty)
            {
                _inner.Emit(context);
            }
        }

        public void Emit(object instance)
        {
            if (Enabled)
            {
                _inner.Emit(instance);
            }
        }

        public void Dispose() => _inner.Dispose();

        sealed class Context : IDisposable
        {
            private readonly IExtendedXmlSerializerConfig _configuration;
            private readonly XmlWriter _writer;
            private readonly object _instance;
            private readonly IDisposable _inner;
            private readonly IDisposable _complete;

            public Context(IExtendedXmlSerializerConfig configuration, XmlWriter writer, object instance,
                           IDisposable inner, Action complete) : this(inner, new DelegatedDisposable(complete))
            {
                _configuration = configuration;
                _writer = writer;
                _instance = instance;
            }

            public Context(IDisposable inner, IDisposable complete)
            {
                _inner = inner;
                _complete = complete;
            }

            public void Dispose()
            {
                _configuration.WriteObject(_writer, _instance);
                _inner.Dispose();
                _complete.Dispose();
            }
        }
    }
}