using System;
using ExtendedXmlSerialization.ProcessModel;

namespace ExtendedXmlSerialization.Instructions
{
    class DeferredInstruction : IInstruction
    {
        private readonly Lazy<IInstruction> _source;

        public DeferredInstruction(Func<IInstruction> source) : this(new Lazy<IInstruction>(source)) {}

        public DeferredInstruction(Lazy<IInstruction> source)
        {
            _source = source;
        }

        public void Execute(IProcess services) => _source.Value.Execute(services);
    }
}