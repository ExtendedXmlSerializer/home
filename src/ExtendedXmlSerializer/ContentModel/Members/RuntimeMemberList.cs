using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class RuntimeMemberList : IRuntimeMemberList
	{
		readonly Func<IMemberSerializer, bool> _specification;
		readonly IMemberSerializer[]           _properties, _contents;
		readonly IRuntimeSerializer[]          _runtime;

		// ReSharper disable once TooManyDependencies
		public RuntimeMemberList(Func<IMemberSerializer, bool> specification, IEnumerable<IMemberSerializer> properties,
		                         IEnumerable<IRuntimeSerializer> runtime, IEnumerable<IMemberSerializer> contents)
		{
			_specification = specification;
			_properties    = properties.ToArray();
			_runtime       = runtime.ToArray();
			_contents      = contents.ToArray();
		}

		public ImmutableArray<IMemberSerializer> Get(object parameter)
		{
			var runtime = Runtime(parameter)
				.ToArray();
			var runtimeProperties = runtime.Where(_specification)
			                               .ToArray();

			var properties = _properties.Concat(runtimeProperties)
			                            .OrderBy(x => x.Profile.Order);
			var contents = _contents.Concat(runtime.Except(runtimeProperties))
			                        .OrderBy(x => x.Profile.Order);
			var result = properties.Concat(contents)
			                       .ToImmutableArray();
			return result;
		}

		IEnumerable<IMemberSerializer> Runtime(object parameter)
		{
			var length = _runtime.Length;
			var result = new IMemberSerializer[length];
			for (var i = 0; i < length; i++)
			{
				result[i] = _runtime[i]
					.Get(parameter);
			}

			return result;
		}
	}
}