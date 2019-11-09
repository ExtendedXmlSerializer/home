using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	public interface IMemberAccess : ISpecification<object>
	{
		ISpecification<object> Instance { get; }

		object Get(object instance);

		void Assign(object instance, object value);
	}
}