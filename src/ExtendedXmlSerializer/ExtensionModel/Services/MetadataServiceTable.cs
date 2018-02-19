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

using ExtendedXmlSerializer.Core.Sources;
using System.Collections.Generic;
using System.Linq;

namespace ExtendedXmlSerializer.ExtensionModel.Services
{
	interface IRegisteredService<out T> : IRegistration, IService<T> {}

	/*interface IServiceTable<TKey, TValue> : ITableSource<TKey, IService<TValue>>, IRegistration {}

	interface IMetadata<out T> : ISpecificationSource<MemberInfo, IService<T>> {}

	interface IMetadataTable<T> : IServiceTable<MemberInfo, T> {}

	class MetadataTable<T, TInstance> : MetadataTable<T> where TInstance : IMetadata<T>
	{
		public MetadataTable() : this(new TypeServices<T>(), new MemberServices<T>()) { }

		public MetadataTable(IServiceTable<TypeInfo, T> types, IServiceTable<MemberInfo, T> members) : this(types, members, new CompositeRegistration(types, members)) {}
		public MetadataTable(IServiceTable<TypeInfo, T> types, IServiceTable<MemberInfo, T> members, IRegistration registration) : base(types, members, registration) {}

		public override IServiceRepository Get(IServiceRepository parameter) => base.Get(parameter).RegisterInstance(typeof(TInstance), this);
	}

	class MetadataTable<T> : ReflectionModel.MetadataTable<IService<T>>, IMetadataTable<T>
	{
		readonly IRegistration _registration;

		public MetadataTable(IServiceTable<TypeInfo, T> types, IServiceTable<MemberInfo, T> members, IRegistration registration)
			: base(types, members) => _registration = registration;

		public virtual IServiceRepository Get(IServiceRepository parameter) => _registration.Get(parameter);
	}

	class TypeServices<T> : ServiceTable<TypeInfo, T>
	{
		public TypeServices() : base(ReflectionModel.Defaults.TypeComparer) {}
	}

	class MemberServices<T> : ServiceTable<MemberInfo, T>
	{
		public MemberServices() : base(MemberComparer.Default) {}
	}

	class ServiceTable<TKey, TValue> : DecoratedTable<TKey, IService<TValue>>, IServiceTable<TKey, TValue>
	{
		readonly IRegistration _registration;

		public ServiceTable(IEqualityComparer<TKey> comparer) : this(new Dictionary<TKey, IService<TValue>>(comparer)) {}

		public ServiceTable(IDictionary<TKey, IService<TValue>> store)
			: this(new TableSource<TKey, IService<TValue>>(store), store) {}

		public ServiceTable(ITableSource<TKey, IService<TValue>> table, IDictionary<TKey, IService<TValue>> store)
			: this(table, new ItemRegistration<IService<TValue>>(new Values<TKey, IService<TValue>>(store))) {}

		public ServiceTable(ITableSource<TKey, IService<TValue>> table, IRegistration registration)
			: base(table) => _registration = registration;

		public IServiceRepository Get(IServiceRepository parameter) => _registration.Get(parameter);
	}*/

	sealed class ItemRegistration<T> : IRegistration
	{
		readonly IEnumerable<T> _store;

		public ItemRegistration(IEnumerable<T> store) => _store = store;

		public IServiceRepository Get(IServiceRepository parameter) => _store.OfType<IRegistration>().Alter(parameter);
	}
}