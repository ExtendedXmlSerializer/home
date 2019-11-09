using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberSerializations : CacheBase<TypeInfo, IMemberSerialization>, IMemberSerializations
	{
		readonly Func<IEnumerable<IMemberSerializer>, IMemberSerialization> _builder;
		readonly ITypeMembers                                               _members;
		readonly Func<IMember, IMemberSerializer>                           _serializers;

		[UsedImplicitly]
		public MemberSerializations(ITypeMembers members, IMemberSerializers serializers)
			: this(MemberSerializationBuilder.Default.Get, members, serializers.Get) {}

		public MemberSerializations(Func<IEnumerable<IMemberSerializer>, IMemberSerialization> builder,
		                            ITypeMembers members,
		                            Func<IMember, IMemberSerializer> serializers)
		{
			_builder     = builder;
			_members     = members;
			_serializers = serializers;
		}

		protected override IMemberSerialization Create(TypeInfo parameter)
		{
			var members = _members.Get(parameter)
			                      .Select(_serializers)
			                      .ToArray();
			var result = _builder(members);
			return result;
		}
	}
}