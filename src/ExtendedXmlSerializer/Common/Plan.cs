using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
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

    class PlanSelector : IPlan
    {
        readonly ICollection<IPlan> _providers;
        readonly WeakCache<Type, ICollection<IAssignableInstruction>> _placeholders =
            new WeakCache<Type, ICollection<IAssignableInstruction>>(key => new HashSet<IAssignableInstruction>());

        public PlanSelector(ICollection<IPlan> providers)
        {
            _providers = providers;
        }

        public IInstruction For(Type type)
        {
            var contains = _placeholders.Contains(type);
            var placeholders = _placeholders.Get(type);
            if (contains)
            {
                var result = new PlaceholderInstruction();
                placeholders.Add(result);
                return result;
            }

            try
            {
                foreach (var provider in _providers)
                {
                    var result = provider.For(type);
                    if (result != null)
                    {
                        foreach (var instruction in placeholders)
                        {
                            instruction.Assign(result);
                        }
                        return result;
                    }
                }
            }
            finally
            {
                placeholders.Clear();
            }
            return null;
        }

        class PlaceholderInstruction : IAssignableInstruction
        {
            private IInstruction _instruction;
            public void Execute(IServiceProvider services) => _instruction?.Execute(services);

            public void Assign(IInstruction instruction) => _instruction = instruction;
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

    /*class FixedDecoratedPlan : IPlan
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
    }*/


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
