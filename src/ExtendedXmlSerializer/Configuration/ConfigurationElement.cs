// MIT License
//
// Copyright (c) 2016-2018 Wojciech Nagórski
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

using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace ExtendedXmlSerializer.Configuration
{
	public class ConfigurationElement : IConfigurationElement
	{
		readonly IExtensions _extensions;
		readonly IExtend _extend;
		readonly IActivator<IExtendedXmlSerializer> _activator;

		public ConfigurationElement(IConfigurationElement element) : this(element, element) {}

		public ConfigurationElement(IExtensions extensions, IExtend extend)
			: this(extensions, extend, Activator<IExtendedXmlSerializer>.Default) {}

		public ConfigurationElement(IExtensions extensions, IExtend extend, IActivator<IExtendedXmlSerializer> activator)
		{
			_extensions = extensions;
			_extend = extend;
			_activator = activator;
		}

		public ICommand<ISerializerExtension> Add => _extensions.Add;

		public ICommand<ISerializerExtension> Remove => _extensions.Remove;

		public IExtendedXmlSerializer Create() => _activator.Get(_extensions.ToImmutableArray());

		public void Execute(ISerializerExtension parameter)
		{
			_extensions.Add.Execute(parameter);
		}

		public IEnumerator<ISerializerExtension> GetEnumerator() => _extensions.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public ISerializerExtension Get(TypeInfo parameter) => _extend.Get(parameter);
	}
}