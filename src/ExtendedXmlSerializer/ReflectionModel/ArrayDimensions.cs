using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class ArrayDimensions : ItemsBase<int>
	{
		readonly TypeInfo _type;

		public ArrayDimensions(TypeInfo type)
		{
			_type = type;
		}

		public override IEnumerator<int> GetEnumerator()
		{
			var type = _type;
			while (type.IsArray)
			{
				yield return type.GetArrayRank();
				type = type.GetElementType()
				           .GetTypeInfo();
			}
		}
	}
}