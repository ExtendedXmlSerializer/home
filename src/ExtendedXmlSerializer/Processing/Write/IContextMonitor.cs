using ExtendedXmlSerialization.Model.Write;

namespace ExtendedXmlSerialization.Processing.Write
{
    public interface IContextMonitor
    {
        void Update(IContext context);
        IContext Current { get; }
    }

    class ContextMonitor : IContextMonitor
    {
        public void Update(IContext context) => Current = context;

        public IContext Current { get; private set; }
    }
}