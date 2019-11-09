using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using ExtendedXmlSerializer.Core;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	[UsedImplicitly]
	class Migrations : Collection<ICommand<XElement>>, IEnumerable<Action<XElement>>
	{
		public new IEnumerator<Action<XElement>> GetEnumerator()
			=> this.Select<ICommand<XElement>, Action<XElement>>(x => x.Execute)
			       .GetEnumerator();
	}
}