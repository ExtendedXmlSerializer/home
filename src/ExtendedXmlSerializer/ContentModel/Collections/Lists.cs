using System;
using System.Collections;
using System.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Collections
{
	sealed class Lists : ReferenceCacheBase<object, IList>, ILists
	{
		public static Lists Default { get; } = new Lists();

		Lists() : this(AddDelegates.Default) {}

		readonly IAddDelegates _add;

		public Lists(IAddDelegates add) => _add = add;

		protected override IList Create(object parameter) => parameter as IList ?? CreateAdapter(parameter);

		IList CreateAdapter(object parameter)
		{
			var generic = new Generic<object, Action<object, object>, IList>(typeof(ListAdapter<>));
			var typeInfo = parameter.GetType()
			                        .GetTypeInfo();
			var type   = CollectionItemTypeLocator.Default.Get(typeInfo);
			var action = _add.Get(typeInfo);
			var result = generic.Get(type)
			                    .Invoke(parameter, action);
			return result;
		}
	}
}