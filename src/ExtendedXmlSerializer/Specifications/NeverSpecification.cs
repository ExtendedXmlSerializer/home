namespace ExtendedXmlSerialization.Specifications
{
    public class NeverSpecification<T> : ISpecification<T>
    {
        public static NeverSpecification<T> Default { get; } = new NeverSpecification<T>();
        NeverSpecification() {}

        public bool IsSatisfiedBy(T parameter) => false;
    }
}