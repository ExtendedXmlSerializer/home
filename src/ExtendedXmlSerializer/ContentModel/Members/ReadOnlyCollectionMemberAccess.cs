using System.Collections;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class ReadOnlyCollectionMemberAccess : IMemberAccess
	{
		readonly IMemberAccess _access;

		public ReadOnlyCollectionMemberAccess(IMemberAccess access) => _access = access;

		public ISpecification<object> Instance => _access.Instance;

		public bool IsSatisfiedBy(object parameter) => _access.IsSatisfiedBy(parameter);

		public object Get(object instance) => _access.Get(instance);

		public void Assign(object instance, object value)
		{
			var collection = _access.Get(instance);
			foreach (var element in (IEnumerable)value)
			{
				_access.Assign(collection, element);
			}
		}
	}
}