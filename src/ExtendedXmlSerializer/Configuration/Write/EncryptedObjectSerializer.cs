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

using ExtendedXmlSerialization.Processing.Write;

namespace ExtendedXmlSerialization.Configuration.Write
{
    class EncryptedObjectSerializer : IObjectSerializer
    {
        private readonly IEncryptionFactory _encryption;
        private readonly IContextMonitor _monitor;
        private readonly IObjectSerializer _inner;


        public EncryptedObjectSerializer(
            IContextMonitor monitor,
            IEncryptionFactory encryption) : this(monitor, encryption, ObjectSerializer.Default) {}

        public EncryptedObjectSerializer(
            IContextMonitor monitor,
            IEncryptionFactory encryption,
            IObjectSerializer inner)
        {
            _monitor = monitor;
            _encryption = encryption;
            _inner = inner;
        }

        public string Serialize(object instance)
        {
            var text = _inner.Serialize(instance);
            var encryption = _encryption.Get(_monitor.Current);
            var result = encryption?.Encrypt(text) ?? text;
            return result;
        }
    }
}