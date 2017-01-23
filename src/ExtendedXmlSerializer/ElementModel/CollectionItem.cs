using System;

namespace ExtendedXmlSerialization.ElementModel
{
	public class CollectionItem : ContainerElementBase
	{
		public CollectionItem(Func<IElement> element) : base(element) {}
	}
}