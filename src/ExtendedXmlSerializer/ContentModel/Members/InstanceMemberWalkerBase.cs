using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
			var parameter = input.GetType()
			                     .GetTypeInfo();
			var result = Members(input, parameter).Concat(Enumerate(input).Select(_selector).SelectMany(x => x));
			return result;
		}

		IEnumerable<object> Enumerate(object parameter)
		{
			var iterator = _enumerators.For(parameter);
			while (iterator?.MoveNext() ?? false)
			{
				yield return iterator.Current;
			}
		}

		protected virtual IEnumerable<T> Members(object input, TypeInfo parameter)
		{
			var members = _members.Get(parameter);
			var length  = members.Length;

			for (var i = 0; i < length; i++)
			{
				var member = members[i];

				foreach (var item in Yield(member, input))
				{
					yield return item;
				}
			}
		}

		protected abstract IEnumerable<T> Yield(IMember member, object instance);

		protected abstract IEnumerable<T> Yield(object instance);
	}
}