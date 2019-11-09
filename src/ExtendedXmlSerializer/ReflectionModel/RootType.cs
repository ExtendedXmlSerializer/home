using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class RootType : IAlteration<TypeInfo>
	{
		public static RootType Default { get; } = new RootType();

		RootType() {}

		public TypeInfo Get(TypeInfo parameter)
		{
			var result = parameter;
			while (result.IsArray)
			{
				result = result.GetElementType()
				               .GetTypeInfo();
			}

			return result;
		}
	}
}