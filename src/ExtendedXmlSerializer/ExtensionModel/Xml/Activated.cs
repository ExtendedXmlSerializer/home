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
using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	class Activated<T> : Generic<object, T>, IParameterizedSource<IServices, T>, IAlteration<IServiceRepository>
		where T : class
	{
		readonly ISingletonLocator _locator;
		readonly Type _objectType;
		readonly TypeInfo _targetType;

		public Activated(Type objectType, TypeInfo targetType, Type definition) : this(SingletonLocator.Default, objectType,
		                                                                               targetType, definition) {}

		public Activated(ISingletonLocator locator, Type objectType, TypeInfo targetType, Type definition) : base(definition)
		{
			_locator = locator;
			_objectType = objectType;
			_targetType = targetType;
		}

		public T Get(IServices parameter)
		{
			var service = parameter.GetService(_objectType);

			var result = service as T ?? Get(_targetType.Yield()
			                                            .ToImmutableArray())
				             .Invoke(service);
			return result;
		}

		public IServiceRepository Get(IServiceRepository parameter)
		{
			var singleton = _locator.Get(_objectType);
			var result = singleton != null
				             ? parameter.RegisterInstance(_objectType, singleton)
				             : parameter.Register(_objectType);
			return result;
		}
	}
}