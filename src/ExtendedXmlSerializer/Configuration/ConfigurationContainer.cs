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

using ExtendedXmlSerializer.ExtensionModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.Configuration
{
	public class ConfigurationContainer : ContextBase, IConfigurationContainer
	{
		readonly IRootContext _context;
		readonly ITypeConfigurations _types;

		public ConfigurationContainer() : this(DefaultExtensions.Default.ToArray()) {}

		public ConfigurationContainer(params ISerializerExtension[] extensions) : this(new ExtensionCollection(extensions)) {}

		public ConfigurationContainer(IExtensionCollection extensions) : this(new RootContext(extensions)) {}

		public ConfigurationContainer(IRootContext context) : this(context, new TypeConfigurations(context)) {}
		public ConfigurationContainer(ITypeConfigurationContext parent) : this(parent, new TypeConfigurations(parent.Root)) {}

		public ConfigurationContainer(IRootContext context, ITypeConfigurations types) : base(context, context)
		{
			_context = context;
			_types = types;
		}
		public ConfigurationContainer(ITypeConfigurationContext parent, ITypeConfigurations types) : base(parent)
		{
			_context = parent.Root;
			_types = types;
		}

		public ITypeConfiguration Type(TypeInfo type) => _types.Get(type);

		public IConfigurationContainer Extend(ISerializerExtension extension)
		{
			var existing = _context.SingleOrDefault(extension.GetType()
			                                                 .IsInstanceOfType);
			if (existing != null)
			{
				_context.Remove(existing);
			}
			_context.Add(extension);
			return this;
		}

		/*IEnumerator<ISerializerExtension> IEnumerable<ISerializerExtension>.GetEnumerator() => _context.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => _context.GetEnumerator();
		void ICollection<ISerializerExtension>.Add(ISerializerExtension item) => _context.Add(item);
		void ICollection<ISerializerExtension>.Clear() => _context.Clear();
		bool ICollection<ISerializerExtension>.Contains(ISerializerExtension item) => _context.Contains(item);

		void ICollection<ISerializerExtension>.CopyTo(ISerializerExtension[] array, int arrayIndex) =>
			_context.CopyTo(array, arrayIndex);

		bool ICollection<ISerializerExtension>.Remove(ISerializerExtension item) => _context.Remove(item);
		int ICollection<ISerializerExtension>.Count => _context.Count;
		bool ICollection<ISerializerExtension>.IsReadOnly => _context.IsReadOnly;
		public bool Contains<T>() where T : ISerializerExtension => _context.Contains<T>();
		public T Find<T>() where T : ISerializerExtension => _context.Find<T>();*/

		public IEnumerator<ITypeConfiguration> GetEnumerator() => _types.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _context.GetEnumerator();
	}
}