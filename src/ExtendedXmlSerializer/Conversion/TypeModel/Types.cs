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
using System.Xml.Linq;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.Conversion.TypeModel
{
    public class Types : WeakCacheBase<XElement, Type>, ITypes
    {
        public static Types Default { get; } = new Types();
        Types() : this(TypeParser.Default, Identities.Default) {}

        private readonly ITypeParser _parser;
        private readonly IIdentities _identities;

        public Types(ITypeParser parser, IIdentities identities)
        {
            _parser = parser;
            _identities = identities;
        }

        protected override Type Create(XElement parameter)
            => _identities.Get(parameter.Name) ?? FromAttribute(parameter);

        private Type FromAttribute(XElement parameter)
        {
            var value = parameter.Attribute(ExtendedXmlSerializer.Type)?.Value;
            var result = value != null ? _parser.Get(value) : null;
            /*if (result == null)
            {
                throw new SerializationException($"Could not find Type information from provided value: {value}");
            }*/

            return result;
        }
    }
}