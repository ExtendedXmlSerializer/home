using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class InterfaceIdentities : IInterfaceIdentities
	{
		public static InterfaceIdentities Default { get; } = new InterfaceIdentities();

		InterfaceIdentities() : this(AllInterfaces.Default) {}

		readonly IAllInterfaces _interfaces;

		public InterfaceIdentities(IAllInterfaces interfaces) => _interfaces = interfaces;

		public ImmutableArray<Guid> Get(TypeInfo parameter)
			=> _interfaces.Get(parameter)
			              .Select(x => x.GUID)
			              .ToImmutableArray();
	}
}