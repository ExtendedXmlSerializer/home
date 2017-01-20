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

namespace ExtendedXmlSerialization.Legacy
{
	/// <summary>
	/// Interface Extended Xml Serializer
	/// </summary>
	[Obsolete(Support.Message)]
	public interface IExtendedXmlSerializer
	{
		/// <summary>
		/// Serializes the specified <see cref="T:System.Object" /> and returns xml document in string
		/// </summary>
		/// <param name="o">The <see cref="T:System.Object" /> to serialize. </param>
		/// <returns>xml document in string</returns>
		string Serialize(object o);

		/// <summary>
		/// Deserializes the XML document
		/// </summary>
		/// <param name="xml">The XML document</param>
		/// <param name="type">The type of returned object</param>
		/// <returns>deserialized object</returns>
		object Deserialize(string xml, Type type);

		/// <summary>
		/// Deserializes the XML document
		/// </summary>
		/// <typeparam name="T">The type of returned object</typeparam>
		/// <param name="xml">The XML document</param>
		/// <returns>deserialized object</returns>
		T Deserialize<T>(string xml);

		/// <summary>
		/// Gets or sets <see cref="ISerializationToolsFactory"/>
		/// </summary>
		ISerializationToolsFactory SerializationToolsFactory { get; set; }
	}
}