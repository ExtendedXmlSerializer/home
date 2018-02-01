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

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.Configuration
{
	/*public interface ITypeMemberConfigurations : IParameterizedSource<ITypeConfiguration, IReadOnlyDictionary<MemberInfo, IMemberConfiguration>> {}

	sealed class TypeMemberConfigurations : ITypeMemberConfigurations
	{
		public IReadOnlyDictionary<MemberInfo, IMemberConfiguration> Get(ITypeConfiguration parameter)
		{
			throw new System.NotImplementedException();
		}

		sealed class Source : Generic<IContext, ContentModel.Properties.IProperty<string>, ContentModel.Properties.IProperty<int>, IMemberConfiguration>
		{
			public static Source Default { get; } = new Source();
			Source() : base(typeof(MemberConfiguration<,>)) { }
		}
	}*/

	/*public interface IMemberConfigurations : IParameterizedSource<MemberInfo, IMemberConfiguration> {}*/

	// ReSharper disable once UnusedTypeParameter
	sealed class MemberConfigurations<T> : CacheBase<MemberInfo, IMemberConfiguration>, IMemberConfigurations
	{
		readonly ITypeConfigurationContext _parent;
		readonly ConcurrentDictionary<MemberInfo, IMemberConfiguration> _store;

		public MemberConfigurations(ITypeConfigurationContext parent) :
			this(parent, new ConcurrentDictionary<MemberInfo, IMemberConfiguration>()) {}

		public MemberConfigurations(ITypeConfigurationContext parent,
		                            ConcurrentDictionary<MemberInfo, IMemberConfiguration> store)
			: base(store)
		{
			_parent = parent;
			_store = store;
		}

		protected override IMemberConfiguration Create(MemberInfo parameter)
			=> Source.Default
			         .Get(_parent.Get(), MemberDescriptors.Default.Get(parameter)
			                                              .MemberType)
			         .Invoke(_parent, parameter);

		sealed class Source : Generic<ITypeConfigurationContext, MemberInfo, IMemberConfiguration>
		{
			public static Source Default { get; } = new Source();
			Source() : base(typeof(MemberConfiguration<,>)) {}
		}

		public IEnumerator<IMemberConfiguration> GetEnumerator() => _store.Values.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

/*
	sealed class Property<T> : IProperty<T>
	{
		T _value;

		public T Get() => _value;

		public void Assign(T value) => _value = value;
	}
*/
}