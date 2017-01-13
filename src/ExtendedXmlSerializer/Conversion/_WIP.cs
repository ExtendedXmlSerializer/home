using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using ExtendedXmlSerialization.Conversion.ElementModel;
using ExtendedXmlSerialization.Conversion.Members;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion
{
    public interface IConverterSelector : ISelector<IElement, IConverter> {}

    public abstract class ConverterFactoryBase<T> : IConverterOption where T : IElement
    {
        private readonly ISpecification<IElement> _specification;

        protected ConverterFactoryBase() : this(IsTypeSpecification<T>.Default) {}

        protected ConverterFactoryBase(ISpecification<IElement> specification)
        {
            _specification = specification;
        }

        IConverter IParameterizedSource<IElement, IConverter>.Get(IElement parameter) => Create((T) parameter);

        protected abstract IConverter Create(T parameter);

        public bool IsSatisfiedBy(IElement parameter) => _specification.IsSatisfiedBy(parameter);
    }

    public interface IConverterOption : IOption<IElement, IConverter> {}

    public class ConverterSelector : Selector<IElement, IConverter>, IConverterSelector
    {
        public ConverterSelector(params IConverterOption[] options) : base(options) {}

        /*public CompositeConverterFactory(params IMemberFactory[] factories) : this(factories.ToImmutableArray()) {}

        public CompositeConverterFactory(ImmutableArray<IMemberFactory> factories)
        {
            _factories = factories;
        }

        public IMemberConverter Get(IMemberElement parameter)
        {
            foreach (var factory in _factories)
            {
                var member = factory.Get(parameter);
                if (member != null)
                {
                    return member;
                }
            }


            // TODO: Warning? Throw?
            return null;
        }*/
    }
}