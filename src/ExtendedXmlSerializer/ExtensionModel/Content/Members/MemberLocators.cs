using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class MemberLocators : ReferenceCacheBase<TypeInfo, IMetadataLocator>, IMemberLocators
	{
		public static MemberLocators Default { get; } = new MemberLocators();

		MemberLocators() : this(IsSerializableMember.Default.IsSatisfiedBy) {}

		readonly Func<MemberInfo, bool> _specification;

		public MemberLocators(Func<MemberInfo, bool> specification) => _specification = specification;

		protected override IMetadataLocator Create(TypeInfo parameter)
			=> new MetadataLocator(parameter.GetMembers()
			                                .GroupBy(x => x.Name)
			                                .Where(x => x.Count() == 1)
			                                .SelectMany(x => x.ToArray())
			                                .Where(_specification)
			                                .ToImmutableArray());
	}
}