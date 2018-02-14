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
using System.Linq;

namespace ExtendedXmlSerializer.Configuration
{
	/*sealed class Elements2 : ISource<IConfigurationElement>
	{
		readonly IExtensions _extensions;
		readonly IExtend _extend;
		readonly IActivator<IExtendedXmlSerializer> _activator;
		readonly ITypeConfigurations _types;

		public Elements2(IExtensions extensions, IExtend extend, IActivator<IExtendedXmlSerializer> activator, ITypeConfigurations types)
		{
			_extensions = extensions;
			_extend = extend;
			_activator = activator;
			_types = types;
		}

		public IConfigurationElement Get() => new ConfigurationElement(_extensions, _types, _extend, _activator);
	}

	sealed class Elements : IParameterizedSource<Func<ITypeConfigurations>, IConfigurationElement>
	{
		readonly IExtensions _extensions;
		readonly IExtend _extend;
		readonly IActivator<IExtendedXmlSerializer> _activator;
		readonly Func<ITypeConfigurations> _types;

		public Elements(IExtensions extensions, IExtend extend, IActivator<IExtendedXmlSerializer> activator)
		{
			_extensions = extensions;
			_extend = extend;
			_activator = activator;
		}

		public ITypeConfigurations Get()
		{

		}

		public IConfigurationElement Get(Func<ITypeConfigurations> parameter)
		{
			return new ConfigurationElement(_extensions, new DeferredTypes(parameter), _extend, _activator);
		}
	}*/

	public class ConfigurationContainer : ConfigurationElement
	{
		public ConfigurationContainer() : this(DefaultExtensions.Default.ToArray()) {}

		public ConfigurationContainer(params ISerializerExtension[] extensions) : this(ConfigurationElements.Default,
		                                                                               extensions) {}

		public ConfigurationContainer(IConfigurationElements configuration, params ISerializerExtension[] extensions)
			: base(configuration.Get(extensions)) {}
	}
}