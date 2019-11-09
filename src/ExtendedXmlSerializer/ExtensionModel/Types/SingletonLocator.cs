using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	sealed class SingletonLocator : CacheBase<TypeInfo, object>, ISingletonLocator
	{
		public static SingletonLocator Default { get; } = new SingletonLocator();

		SingletonLocator() : this(SingletonCandidates.Default, Singletons.Default) {}

		readonly ISingletonCandidates _candidates;
		readonly ISingletons          _singletons;

		public SingletonLocator(ISingletonCandidates candidates, ISingletons singletons)
		{
			_candidates = candidates;
			_singletons = singletons;
		}

		protected override object Create(TypeInfo parameter)
		{
			var property = _candidates.Select(parameter.GetProperty)
			                          .FirstOrDefault(x => x != null && x.CanRead && x.GetMethod.IsStatic);
			var result = property != null ? _singletons.Get(property) : null;
			return result;
		}
	}
}