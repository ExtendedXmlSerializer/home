using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Collections
{
	sealed class Dimensions : IParameterizedSource<Array, ImmutableArray<int>>
	{
		public static IParameterizedSource<TypeInfo, Func<Array, ImmutableArray<int>>> Defaults { get; }
			= new ReferenceCache<TypeInfo, Func<Array, ImmutableArray<int>>>(x => new Dimensions(x.GetArrayRank()).Get);

		readonly int _dimensions;

		public Dimensions(int dimensions)
		{
			_dimensions = dimensions;
		}

		public ImmutableArray<int> Get(Array parameter) => new Items(parameter, _dimensions).ToImmutableArray();

		sealed class Items : ItemsBase<int>
		{
			readonly Array _array;
			readonly int   _dimensions;

			public Items(Array array, int dimensions)
			{
				_array      = array;
				_dimensions = dimensions;
			}

			public override IEnumerator<int> GetEnumerator()
			{
				for (int i = 0; i < _dimensions; i++)
				{
					yield return _array.GetLength(i);
				}
			}
		}
	}
}