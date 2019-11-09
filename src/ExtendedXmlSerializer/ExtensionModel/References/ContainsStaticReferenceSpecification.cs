using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class ContainsStaticReferenceSpecification : DelegatedSpecification<TypeInfo>, IStaticReferenceSpecification
	{
		public ContainsStaticReferenceSpecification(IDiscoveredTypes types) : base(new Source(types).Get) {}

		sealed class Source : StructureCacheBase<TypeInfo, bool>
		{
			readonly IDiscoveredTypes _types;

			public Source(IDiscoveredTypes types) => _types = types;

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
						if (i != j &&
						    (first.IsInterface || second.IsInterface || first.IsAssignableFrom(second) ||
						     second.IsAssignableFrom(first))
						)
						{
							return true;
						}
					}
				}

				return false;
			}
		}
	}
}