using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class ContainsStaticReferenceSpecification : DelegatedSpecification<TypeInfo>, IStaticReferenceSpecification
	{
		public ContainsStaticReferenceSpecification(IDiscoveredTypes types) : base(new Source(types).Get) {}

		sealed class Source : StructureCacheBase<TypeInfo, bool>
		{
			readonly IDiscoveredTypes           _types;
			readonly ICollectionItemTypeLocator _item;

			public Source(IDiscoveredTypes types) : this(types, CollectionItemTypeLocator.Default) {}

			public Source(IDiscoveredTypes types, ICollectionItemTypeLocator item)
			{
				_types = types;
				_item  = item;
			}

			// ReSharper disable once CognitiveComplexity
			// ReSharper disable once CyclomaticComplexity
			// ReSharper disable once ExcessiveIndentation
			protected override bool Create(TypeInfo parameter)
			{
				var variables = _types.Get(parameter);
				var length    = variables.Length;
				for (var i = 0; i < length; i++)
				{
					var first = variables[i];
					for (var j = 0; j < length; j++)
					{
						var second = variables[j];
						if (i != j)
						{
							if (first.IsInterface || second.IsInterface ||
								first.IsAssignableFrom(second) || second.IsAssignableFrom(first)
								||
								(_item.Get(first)?.IsAssignableFrom(second) ?? false) ||
							    (_item.Get(second)?.IsAssignableFrom(first) ?? false))
							{
								return true;
							}
						}
					}
				}

				return false;
			}
		}
	}
}