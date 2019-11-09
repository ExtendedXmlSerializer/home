using System.Reflection;
using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core.Sources;

// ReSharper disable UnusedTypeParameter

namespace ExtendedXmlSerializer.ExtensionModel.AttachedProperties
{
	class AttachedPropertyConfiguration<TType, TValue> : MemberConfiguration<TType, TValue>
	{
		public AttachedPropertyConfiguration(IMemberConfiguration configuration)
			: base(configuration, ((ISource<MemberInfo>)configuration).Get()) {}
	}
}