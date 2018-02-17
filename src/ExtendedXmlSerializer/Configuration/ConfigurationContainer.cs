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

using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System.Collections.Immutable;

namespace ExtendedXmlSerializer.Configuration
{
	public interface IConfigurationRoot : IActivator<ISerializers> {}

	public class ConfigurationRoot : ConfigurationContainer<ISerializers>, IConfigurationRoot
	{
		public ConfigurationRoot() : base(new ExtensionCollection()) {}
		public ConfigurationRoot(IExtensionCollection extensions) : base(extensions) {}
		public ConfigurationRoot(IActivator<ISerializers> activator) : base(activator) {}
	}

	public interface IConfigurationContainer : IActivator<IExtendedXmlSerializer> {}

	public class ConfigurationContainer : ConfigurationContainer<IExtendedXmlSerializer>, IConfigurationContainer
	{
		public ConfigurationContainer() : base(new ExtensionCollection()) {}

		public ConfigurationContainer(params ISerializerExtension[] extensions)
			: base(ExtensionCollections.Default.Get(extensions.ToImmutableArray())) {}

		public ConfigurationContainer(IExtensionCollection extensions) : base(extensions) {}
	}

	public class ConfigurationContainer<T> : ConfigurationElement, IActivator<T>
	{
		readonly IActivator<T> _activator;

		public ConfigurationContainer(IExtensionCollection extensions) : this(ServiceActivator<T>.Default, extensions) {}

		public ConfigurationContainer(IServiceActivator<T> activator, IExtensionCollection extensions)
			: this(new Activators<T>(activator).Get(extensions)) {}

		public ConfigurationContainer(IActivator<T> activator) : base(activator) => _activator = activator;

		public T Get() => _activator.Get();
	}
}