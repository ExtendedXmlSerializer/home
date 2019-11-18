using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Coercion
{
	readonly struct CoercerParameter
	{
		public CoercerParameter(object instance, TypeInfo targetType)
		{
			Instance   = instance;
			TargetType = targetType;
		}

		public object Instance { get; }
		public TypeInfo TargetType { get; }
	}
}