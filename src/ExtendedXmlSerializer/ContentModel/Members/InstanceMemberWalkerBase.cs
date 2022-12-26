using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	abstract class InstanceMemberWalkerBase<T> : ObjectWalkerBase<object, IEnumerable<T>>
	{
		readonly ITypeMembers                 _members;
		readonly IEnumeratorStore             _enumerators;
		readonly Func<object, IEnumerable<T>> _selector;

		protected InstanceMemberWalkerBase(ITypeMembers members, IEnumeratorStore enumerators, object root) : base(root)
		{
			_members     = members;
			_enumerators = enumerators;
			_selector    = Yield;
		}

		protected override IEnumerable<T> Select(object input)
		{
			var parameter = input.GetType();
			var result    = Members(input, parameter).Concat(Enumerate(input).Select(_selector).SelectMany(x => x));
			return result;
		}

		protected virtual IEnumerable<T> Members(object input, Type parameter)
		{
			var members = _members.Get(parameter);
			for (var i = 0; i < members.Length; i++)
			{
				foreach (var item in Yield(members[i], input))
				{
					yield return item;
				}
			}
		}

		IEnumerable<object> Enumerate(object parameter)
		{
			var iterator = _enumerators.For(parameter);
			while (iterator?.MoveNext() ?? false)
			{
				yield return iterator.Current;
			}
		}

		protected abstract IEnumerable<T> Yield(IMember member, object instance);

		protected abstract IEnumerable<T> Yield(object instance);
	}
}