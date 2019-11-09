using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberOrder : StructureCacheBase<MemberInfo, int>, IMemberOrder
	{
		readonly IDictionary<MemberInfo, int>          _store;
		readonly IParameterizedSource<MemberInfo, int> _source;

		public MemberOrder(IDictionary<MemberInfo, int> store, IParameterizedSource<MemberInfo, int> source)
		{
			_store  = store;
			_source = source;
		}

		protected override int Create(MemberInfo parameter) => _store.GetStructure(parameter) ?? _source.Get(parameter);
	}
}