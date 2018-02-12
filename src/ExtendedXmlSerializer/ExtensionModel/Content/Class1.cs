using ExtendedXmlSerializer.ContentModel;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	sealed class ActivatedSerializer : ActivatedAdapter<IContentSerializer<object>, GenerializedContentSerializer<object>>
	{
		public ActivatedSerializer(Type implementationType, TypeInfo argumentType) : base(implementationType, argumentType) { }
	}
}
