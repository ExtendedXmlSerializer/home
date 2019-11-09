using System.Collections.Generic;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class Encounters : IEncounters
	{
		readonly ISpecification<object>          _specification;
		readonly IDictionary<object, Identifier> _store;

		public Encounters(IDictionary<object, Identifier> store) :
			this(new InstanceConditionalSpecification(), store) {}

		public Encounters(ISpecification<object> specification, IDictionary<object, Identifier> store)
		{
			_specification = specification;
			_store         = store;
		}

		public bool IsSatisfiedBy(object parameter) => _specification.IsSatisfiedBy(parameter);

		public Identifier? Get(object parameter) => _store.GetStructure(parameter);
	}
}