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

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	class Metadata<TMember, T> : TableSource<TMember, T>, ISerializerExtension where TMember : MemberInfo
	{
		readonly IDictionary<TMember, T> _store;

		public Metadata(IEqualityComparer<TMember> comparer) : this(new ConcurrentDictionary<TMember, T>(comparer)) {}

		public Metadata(IDictionary<TMember, T> store) : base(store) => _store = store;


		public IServiceRepository Get(IServiceRepository parameter) => _store.Values.OfType<IAlteration<IServiceRepository>>()
		                                                                     .Aggregate(parameter,
		                                                                                (repository, serializer) =>
			                                                                                serializer.Get(repository));

		public void Execute(IServices parameter)
		{
			foreach (var pair in _store.ToArray())
			{
				if (pair.Value is IParameterizedSource<IServices, T> serializer)
				{
					_store[pair.Key] = serializer.Get(parameter);
				}
			}
		}
	}
}