using System.Collections;
using System.Collections.Generic;
using ExtendedXmlSerialization.ConverterModel.Collections;
using ExtendedXmlSerialization.ConverterModel.Members;

namespace ExtendedXmlSerialization.ConverterModel
{
	class ContainerDefinitions : IContainerDefinitions
	{
		readonly IContainers _containers;

		public ContainerDefinitions(IContainers containers)
		{
			_containers = containers;
		}

		public IEnumerator<ContainerDefinition> GetEnumerator()
		{
			yield return new ContainerDefinition(ElementOption.Default, WellKnownContent.Default);
			yield return new ContainerDefinition(ArrayElementOption.Default, new ArrayContentOption(_containers));
			yield return new ContainerDefinition(Elements.Default, new CollectionContentOption(_containers));
			yield return new ContainerDefinition(Elements.Default, new MemberedContentOption(new Members.Members(_containers)));
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}