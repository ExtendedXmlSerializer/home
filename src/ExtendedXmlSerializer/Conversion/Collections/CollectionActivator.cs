using System.Collections;
using System.Reflection;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Collections
{
	class CollectionActivator : DecoratedActivator
	{
		readonly IConverter _context;
		readonly IAddDelegates _add;

		public CollectionActivator(IActivator activator, IConverter context, IAddDelegates add) : base(activator)
		{
			_context = context;
			_add = add;
		}

		public override object Get(IReader parameter)
		{
			var result = base.Get(parameter);
			var list = result as IList ?? new ListAdapter(result, _add.Get(result.GetType().GetTypeInfo()));
			var items = parameter.Items();
			while (items.MoveNext())
			{
				list.Add(_context.Get(parameter));
			}
			/*foreach (var _ in parameter.Items())
			{
				list.Add(_context.Yield(parameter));
			}*/
			return result;
		}
	}
}