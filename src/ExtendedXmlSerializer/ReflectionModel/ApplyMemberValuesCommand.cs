using System.Collections.Immutable;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class ApplyMemberValuesCommand : ICommand<object>
	{
		readonly IMemberAccessors             _accessors;
		readonly ImmutableArray<IMember>      _members;
		readonly ITableSource<string, object> _values;

		public ApplyMemberValuesCommand(IMemberAccessors accessors, ImmutableArray<IMember> members,
		                                ITableSource<string, object> values)
		{
			_accessors = accessors;
			_members   = members;
			_values    = values;
		}

		public void Execute(object parameter)
		{
			foreach (var member in _members)
			{
				var isSatisfiedBy = _values.IsSatisfiedBy(member.Name);
				if (isSatisfiedBy)
				{
					var access = _accessors.Get(member);
					var value  = _values.Get(member.Name);
					access.Assign(parameter, value);
				}
			}
		}
	}
}