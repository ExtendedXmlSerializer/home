// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
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
using System.Xml.Linq;

namespace ExtendedXmlSerialization
{
    /// <summary>
    /// Base class for castom object serializer
    /// </summary>
    /// <typeparam name="T">The type of object to serialize and deserialize</typeparam>
    public abstract class AbstractCustomSerializator<T> : ICustomSerializator<T>
    {
        /// <summary>
        /// Gets the type of object to deserialize
        /// </summary>
        public Type Type => typeof(T);

        object ICustomSerializator.ReadObject(XElement element)
        {
           return Read(element);
        }

        void ICustomSerializator.WriteObject(XmlWriter writer, object obj)
        {
            Write(writer, (T)obj);
        }
        
        /// <summary>
        /// Read <see cref="XElement"/> and return object
        /// </summary>
        /// <param name="element">The node to read</param>
        /// <returns>The object</returns>
        public abstract T Read(XElement element);

        /// <summary>
        /// Write xml of object to <see cref="XmlWriter"/>
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write object</param>
        /// <param name="obj">The object to write</param>
        public abstract void Write(XmlWriter writer, T obj);

    }
}
