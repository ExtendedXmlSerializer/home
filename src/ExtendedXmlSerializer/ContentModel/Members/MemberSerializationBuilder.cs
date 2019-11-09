using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberSerializationBuilder : IParameterizedSource<IEnumerable<IMemberSerializer>, IMemberSerialization>
	{
		readonly static Func<IMemberSerializer, bool> Property =
			IsTypeSpecification<PropertyMemberSerializer>.Default.IsSatisfiedBy;

		public static MemberSerializationBuilder Default { get; } = new MemberSerializationBuilder();

		MemberSerializationBuilder() : this(Property) {}

		readonly Func<IMemberSerializer, bool> _property;

		public MemberSerializationBuilder(Func<IMemberSerializer, bool> property) => _property = property;

		public IMemberSerialization Get(IEnumerable<IMemberSerializer> parameter)
		{
			var members = parameter.Fixed();
			var properties = members.Where(_property)
			                        .ToArray();
			var runtime = members.OfType<IRuntimeSerializer>()
			                     .ToArray();
			var contents = members.Except(properties.Concat(runtime))
			                      .ToArray();
			var list = runtime.Any()
				           ? new RuntimeMemberList(_property, properties, runtime, contents)
				           : (IRuntimeMemberList)new FixedRuntimeMemberList(properties.Concat(contents)
				                                                                      .ToImmutableArray());
			var all = properties.Concat(runtime)
			                    .Concat(contents)
			                    .OrderBy(x => x.Profile.Order)
			                    .ToImmutableArray();
			var result = new MemberSerialization(list, all);
			return result;
		}
	}
}