using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtendedXmlSerialization.Cache;

namespace ExtendedXmlSerialization.Common
{
	public interface IPlanMaker
    {
        IPlan Make();
    }

	class CachePlanAlteration : IAlteration<IPlan>
    {
        public static CachePlanAlteration Default { get; } = new CachePlanAlteration();
        CachePlanAlteration() {}

        public IPlan Get(IPlan parameter) => new CachedPlan(parameter);
    }
class CachedPlan : WeakCache<Type, IInstruction>, IPlan
    {
        public CachedPlan(IPlan inner) : base(inner.For) {}

        public IInstruction For(Type type) => Get(type);
    }

	class SelectFirstAssignedPlan : IPlan
    {
        readonly ICollection<IPlan> _providers;

        public SelectFirstAssignedPlan(ICollection<IPlan> providers)
        {
            _providers = providers;
        }

        public IInstruction For(Type type)
        {
            foreach (var provider in _providers)
            {
                var instruction = provider.For(type);
                if (instruction != null)
                {
                    return instruction;
                }
            }
            return null;
        }
    }

    class FixedPlan : IPlan
    {
        private readonly IInstruction _instruction;

        public FixedPlan(IInstruction instruction)
        {
            _instruction = instruction;
        }

        public IInstruction For(Type type) => _instruction;
    }

    class DecoratedPlan : IPlan
    {
        private readonly IPlan _plan;

        public DecoratedPlan(IPlan plan)
        {
            _plan = plan;
        }

        public virtual IInstruction For(Type type) => _plan.For(type);
    }

    class FixedDecoratedPlan : IPlan
    {
        private readonly IPlan _plan;
        private readonly Type _type;

        public FixedDecoratedPlan(IPlan plan, Type type)
        {
            _plan = plan;
            _type = type;
        }

        public IInstruction For(Type type) => Get();

        public IInstruction Get() => _plan.For(_type);
    }


    public interface IPlan
    {
        IInstruction For(Type type);
    }

    class ConditionalPlan : ConditionalPlanBase
    {
        private readonly IPlan _builder;

        public ConditionalPlan(Func<Type, bool> specification, IPlan builder) : base(specification)
        {
            _builder = builder;
        }

        protected override IInstruction Plan(Type type) => _builder.For(type);
    }

    abstract class ConditionalPlanBase : IPlan
    {
        private readonly Func<Type, bool> _specification;

        protected ConditionalPlanBase(Func<Type, bool> specification)
        {
            _specification = specification;
        }

        public IInstruction For(Type type) => _specification(type) ? Plan(type) : null;
        protected abstract IInstruction Plan(Type type);
    }
}
