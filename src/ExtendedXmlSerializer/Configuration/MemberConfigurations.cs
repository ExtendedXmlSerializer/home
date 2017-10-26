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

using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.Configuration
{
	sealed class MemberConfigurations<T> : CacheBase<MemberInfo, IMemberConfiguration>, IMemberConfigurations
	{
		readonly IRootContext _root;
		readonly ConcurrentDictionary<MemberInfo, IMemberConfiguration> _store;

		public MemberConfigurations(IRootContext root) :
			this(root, new ConcurrentDictionary<MemberInfo, IMemberConfiguration>()) {}

		public MemberConfigurations(IRootContext root,
		                                ConcurrentDictionary<MemberInfo, IMemberConfiguration> store)
			: base(store)
		{
			_root = root;
			_store = store;
		}

		protected override IMemberConfiguration Create(MemberInfo parameter)
		{
			var type = Support<T>.Key;
			var configuration = _root.Types.Get(type);
			var memberType = MemberDescriptors.Default.Get(parameter).MemberType;
			var result = Source.Default.Get(type, memberType)
			                   .Invoke(configuration, parameter);
			return result;
		}

		sealed class Source : Generic<ITypeConfiguration, MemberInfo, IMemberConfiguration>
		{
			public static Source Default { get; } = new Source();
			Source() : base(typeof(MemberConfiguration<,>)) {}
		}

		public IEnumerator<IMemberConfiguration> GetEnumerator() => _store.Values.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}