using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class Entities : CacheBase<TypeInfo, IEntity>, IEntities
	{
		readonly IEntityMembers        _registered;
		readonly IMemberConverters     _converters;
		readonly IMemberSerializations _members;

		public Entities(IEntityMembers registered, IMemberConverters converters, IMemberSerializations members)
		{
			_registered = registered;
			_converters = converters;
			_members    = members;
		}

		protected override IEntity Create(TypeInfo parameter)
		{
			var memberInfo = _registered.Get(parameter);
			var result = memberInfo != null
				             ? new Entity(_converters.Get(memberInfo), Locate(parameter, memberInfo))
				             : null;
			return result;
		}

		IMemberSerializer Locate(TypeInfo parameter, MemberInfo memberInfo)
		{
			var comparer = /*new MemberComparer(ImplementedTypeComparer.Default)*/MemberComparer.Default;

			var members = _members.Get(parameter)
			                      .Get();
			var length = members.Length;
			for (var i = 0; i < length; i++)
			{
				var member = members[i];

				if (comparer.Equals(member.Profile.Metadata, memberInfo))
				{
					return member;
				}
			}

			return null;
		}
	}
}