using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class AddItemsCommand : ICommand<object>
	{
		readonly IList<object> _list;

		public AddItemsCommand(IList<object> list) => _list = list;

		public void Execute(object parameter)
		{
			if (parameter is IDictionary dictionary)
			{
				foreach (var entry in _list.OfType<DictionaryEntry>())
				{
					dictionary.Add(entry.Key, entry.Value);
				}
			}
			else if (IsCollectionTypeSpecification.Default.IsSatisfiedBy(parameter.GetType()))
			{
				var list = Lists.Default.Get(parameter);
				foreach (var item in _list)
				{
					list.Add(item);
				}
			}
		}
	}
}