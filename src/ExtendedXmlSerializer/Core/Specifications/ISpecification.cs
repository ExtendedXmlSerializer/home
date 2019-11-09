namespace ExtendedXmlSerializer.Core.Specifications
{
	public interface ISpecification<in T>
	{
		bool IsSatisfiedBy(T parameter);
	}
}