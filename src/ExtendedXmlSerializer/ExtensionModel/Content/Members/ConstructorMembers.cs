using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class ConstructorMembers : CacheBase<ConstructorInfo, ImmutableArray<IMember>?>, IConstructorMembers
	{
		readonly IMembers            _members;
		readonly IMemberLocators     _locator;
		readonly Func<IMember, bool> _member;

		public ConstructorMembers(IMembers members, IMemberLocators locator, IMemberSpecification member)
			: this(members, locator, member.IsSatisfiedBy) {}

		ConstructorMembers(IMembers members, IMemberLocators locator, Func<IMember, bool> member)
		{
			_members = members;
			_locator = locator;
			_member  = member;
		}

		protected override ImmutableArray<IMember>? Create(ConstructorInfo parameter)
		{
			var type = parameter.DeclaringType.GetTypeInfo();
			if (!IsGenericDictionarySpecification.Default.IsSatisfiedBy(type))
				// A bit of a hack to circumvent https://github.com/ExtendedXmlSerializer/home/issues/134
				// might want to pretty this up at some point.
			{
				var source = parameter.GetParameters();
				if (source.Length > 0 || IsCollectionTypeSpecification.Default.IsSatisfiedBy(type)
				) // Turning into a game of Jenga here.
				{
					var result = source.Select(x => x.Name)
					                   .Select(_locator.Get(type).Get)
					                   .Where(x => x.HasValue)
					                   .Select(x => x.Value)
					                   .Select(_members.Get)
					                   .Where(_member)
					                   .OrderBy(x => x.Order)
					                   .Select(x => new ParameterizedMember(x))
					                   .ToImmutableArray<IMember>();
					if (result.Length == source.Length)
					{
						return result;
					}
				}
			}

			return null;
		}
	}
}