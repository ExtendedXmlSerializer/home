using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class XmlInnerContentActivation : IInnerContentActivation
	{
		readonly IActivation                                           _activation;
		readonly IParameterizedSource<TypeInfo, IXmlContentsActivator> _activator;

		[UsedImplicitly]
		public XmlInnerContentActivation(IActivation activation) :
			this(activation, XmlContentsActivatorSelector.Default) {}

		public XmlInnerContentActivation(IActivation activation,
		                                 IParameterizedSource<TypeInfo, IXmlContentsActivator> activator)
		{
			_activation = activation;
			_activator  = activator;
		}

		public IInnerContentActivator Get(TypeInfo parameter)
			=> new XmlInnerContentActivator(_activation.Get(parameter), _activator.Get(parameter));
	}
}