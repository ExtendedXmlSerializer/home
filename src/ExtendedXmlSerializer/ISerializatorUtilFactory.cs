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

namespace ExtendedXmlSerialization
{
    /// <summary>
    /// The factory for creating IMigrationMap and ISerializableModel 
    /// </summary>
    public interface ISerializatorUtilFactory
    {
        /// <summary>
        /// Gets <see cref="IMigrationMap" /> for particualr type
        /// </summary>
        /// <param name="type">The type of object to migration</param>
        /// <returns>The <see cref="IMigrationMap"/></returns>
        IMigrationMap GetMigrationMap(Type type);

        /// <summary>
        /// Gets <see cref="IMigrationMap{T}" /> for particualr type
        /// </summary>
        /// <typeparam name="T">The type of object to migration</typeparam>
        /// <returns>The <see cref="IMigrationMap{T}"/></returns>
        IMigrationMap<T> GetMigrationMap<T>();

        /// <summary>
        /// Gets <see cref="ISerializableModel"/> for particular type
        /// </summary>
        /// <param name="type">The type of object to serialization or deserialization</param>
        /// <returns>The <see cref="ISerializableModel"/></returns>
        ISerializableModel GetSerializableModel(Type type);

        /// <summary>
        /// Gets <see cref="ISerializableModel{T}"/> for particular type
        /// </summary>
        /// <typeparam name="T">The type of object to serialization or deserialization</typeparam>
        /// <returns>The <see cref="ISerializableModel{T}"/></returns>
        ISerializableModel<T> GetSerializableModel<T>();
    }
}
