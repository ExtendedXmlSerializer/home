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

using ExtendedXmlSerializer.Core.Collections;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel;
using System;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.Configuration
{
	public interface IActivator<out T> : IConfigurationElement, ISource<T> {}

	sealed class Activators<T> : IActivators<T>
	{
		readonly IParameterizedSource<IEnumerable<ISerializerExtension>, T> _activator;

		public Activators(IServiceActivator<T> activator) : this(activator.In(ImmutableArrayCoercer<ISerializerExtension>
			                                                                      .Default)) {}

		public Activators(IParameterizedSource<IEnumerable<ISerializerExtension>, T> activator) => _activator = activator;

		public IActivator<T> Get(IExtensionCollection parameter)
		{
			var elements   = new ExtensionElements(parameter);
			var extend     = new Extend(elements);
			var extensions = new Extensions(elements, extend);

			var factory = _activator.Build(parameter);

			var types = new Types();
			var result = new Activator<T>(factory, types, extensions);
			result.Services(new ConfiguredTypes(types, extensions), factory);

			return result;
		}
	}

	public class Activator<T> : ConfigurationElement, IActivator<T>
	{
		readonly Func<T> _source;

		public Activator(Func<T> source, ITypes types, IExtensions extensions) : base(types, extensions) => _source = source;

		public T Get() => _source();
	}
}