using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberDefaults : CacheBase<MemberInfo, object>
	{
		readonly static GetterFactory GetterFactory = GetterFactory.Default;

		readonly object         _instance;
		readonly IGetterFactory _getter;

		public MemberDefaults(object instance) : this(instance, GetterFactory) {}

		public MemberDefaults(object instance, IGetterFactory getter)
		{
			_instance = instance;
			_getter   = getter;
		}

		protected override object Create(MemberInfo parameter) => _getter.Get(parameter)
		                                                                 .Invoke(_instance);
	}
}