using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ExtensionModel.Content.Members;
using ExtendedXmlSerializer.ExtensionModel.Xml.Classic;

namespace ExtendedXmlSerializer.Configuration
{
	public static class EmitBehaviors
	{
		public static IEmitBehavior Always { get; } =
			new EmitBehavior(new AllowedSpecificationAlteration(AlwaysEmitMemberSpecification.Default));

		public static IEmitBehavior NotDefault { get; } =
			new EmitBehavior(new AllowedSpecificationAlteration(AllowAssignedValues.Default));

		public static IEmitBehavior Classic { get; } =
			new EmitBehavior(new AddAlteration(ClassicAllowedMemberValues.Default));

		public static IEmitBehavior Assigned { get; } =
			new EmitBehavior(new AddAlteration(AllowedAssignedInstanceValues.Default));
	}
}