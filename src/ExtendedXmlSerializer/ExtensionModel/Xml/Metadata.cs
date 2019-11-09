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

		public IServiceRepository Get(IServiceRepository parameter) => _store
		                                                               .Values.OfType<IAlteration<IServiceRepository>>()
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