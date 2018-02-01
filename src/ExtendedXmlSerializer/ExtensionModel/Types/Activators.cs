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

using System;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	sealed class Activators : IActivators
	{
		public static Activators Default { get; } = new Activators();
		Activators() : this(DefaultActivators.Default, SingletonLocator.Default) {}

		readonly IActivators _activators;
		readonly ISingletonLocator _locator;

		public Activators(IActivators activators, ISingletonLocator locator)
		{
			_activators = activators;
			_locator = locator;
		}

		public IActivator Get(Type parameter)
		{
			var singleton = _locator.Get(parameter);
			var activator = _activators.Build(parameter);
			var result = singleton != null ? new Activator(_activators.Build(parameter), singleton) : activator();
			return result;
		}

		sealed class Activator : IActivator
		{
			readonly Func<IActivator> _activator;
			readonly object _singleton;

			public Activator(Func<IActivator> activator, object singleton)
			{
				_activator = activator;
				_singleton = singleton;
			}

			public object Get()
			{
				try
				{
					return _activator().Get();
				}
				catch (Exception)
				{
					return _singleton;
				}
			}
		}
	}
}