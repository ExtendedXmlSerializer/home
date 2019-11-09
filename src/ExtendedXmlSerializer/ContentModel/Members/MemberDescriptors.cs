using System;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberDescriptors : StructureCacheBase<MemberInfo, MemberDescriptor>
	{
		public static MemberDescriptors Default { get; } = new MemberDescriptors();

		MemberDescriptors() {}

		protected override MemberDescriptor Create(MemberInfo parameter)
		{
			switch (parameter.MemberType)
			{
				case MemberTypes.Property:
					return new MemberDescriptor((PropertyInfo)parameter);
				case MemberTypes.Field:
					return new MemberDescriptor((FieldInfo)parameter);
				case MemberTypes.TypeInfo:
				case MemberTypes.NestedType:
					return new MemberDescriptor((TypeInfo)parameter);
			}

			throw new InvalidOperationException($"{parameter} is not a valid member metadata type.");
		}
	}
}