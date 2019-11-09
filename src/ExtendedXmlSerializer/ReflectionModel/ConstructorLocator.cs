using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class ConstructorLocator : ReferenceCacheBase<TypeInfo, ConstructorInfo>, IConstructorLocator
	{
		public static ConstructorLocator Default { get; } = new ConstructorLocator();

		ConstructorLocator() : this(ValidConstructorSpecification.Default, Constructors.Default) {}

		readonly IValidConstructorSpecification _specification;
		readonly IConstructors                  _constructors;

		public ConstructorLocator(IValidConstructorSpecification specification, IConstructors constructors)
		{
			_specification = specification;
			_constructors  = constructors;
		}

		protected override ConstructorInfo Create(TypeInfo parameter)
		{
			var constructors = _constructors.Get(parameter)
			                                .ToImmutableArray();
			var length = constructors.Length;
			for (var i = 0; i < length; i++)
			{
				var constructor = constructors[i];
				if (_specification.IsSatisfiedBy(constructor))
				{
					return constructor;
				}
			}

			return null;
		}
	}
}