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

using System.Collections;
using System.Collections.Generic;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Xml;

namespace ExtendedXmlSerializer.ConfigurationModel
{
	sealed class RootContext : IRootContext
	{
		readonly IExtensionCollection _extensions;
		readonly IServicesFactory _factory;

		public RootContext(IExtensionCollection extensions) : this(extensions, ServicesFactory.Default) {}

		public RootContext(IExtensionCollection extensions, IServicesFactory factory)
		{
			_extensions = extensions;
			_factory = factory;
		}

		public IRootContext Root => this;

		public IExtendedXmlSerializer Create()
		{
			using (var services = _factory.Get(_extensions))
			{
				var result = services.Get<IExtendedXmlSerializer>();
				return result;
			}
		}

		IEnumerator<ISerializerExtension> IEnumerable<ISerializerExtension>.GetEnumerator() => _extensions.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => _extensions.GetEnumerator();
		void ICollection<ISerializerExtension>.Add(ISerializerExtension item) => _extensions.Add(item);
		void ICollection<ISerializerExtension>.Clear() => _extensions.Clear();
		bool ICollection<ISerializerExtension>.Contains(ISerializerExtension item) => _extensions.Contains(item);

		void ICollection<ISerializerExtension>.CopyTo(ISerializerExtension[] array, int arrayIndex) =>
			_extensions.CopyTo(array, arrayIndex);

		bool ICollection<ISerializerExtension>.Remove(ISerializerExtension item) => _extensions.Remove(item);
		int ICollection<ISerializerExtension>.Count => _extensions.Count;
		bool ICollection<ISerializerExtension>.IsReadOnly => _extensions.IsReadOnly;
		bool IExtensionCollection.Contains<T>() => _extensions.Contains<T>();
		T IExtensionCollection.Find<T>() => _extensions.Find<T>();
	}
}