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

using ExtendedXmlSerialization.Model.Write;

namespace ExtendedXmlSerialization.Processing.Write
{
    class NamespaceEmitter : INamespaceEmitter
    {
        /*private const string Prefix = "xmlns";
        private readonly XmlWriter _writer;
        private readonly INamespaces _namespaces;

        public NamespaceEmitter(XmlWriter writer) : this(writer, DefaultNamespaces.Default) {}

        public NamespaceEmitter(XmlWriter writer, INamespaces namespaces)
        {
            _writer = writer;
            _namespaces = namespaces;
        }

        public void Execute(object instance)
        {
            var list = _namespaces.Get(instance);
            foreach (var pair in list)
            {
                _writer.WriteAttributeString(Prefix, pair.Prefix ?? string.Empty, null, pair.Identifier?.ToString());
            }
        }*/
        public void Execute(IRoot root) {}
    }
}