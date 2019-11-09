using System;
using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.Coercion
{
	sealed class FrameworkCoercer : CoercerBase<IConvertible, object>
	{
		readonly static ISpecification<TypeInfo> Supported = new ContainsSpecification<TypeInfo>(
		                                                                                         new
			                                                                                         HashSet<TypeInfo
			                                                                                         >(new[]
			                                                                                         {
				                                                                                         typeof(bool),
				                                                                                         typeof(char),
				                                                                                         typeof(sbyte),
				                                                                                         typeof(byte),
				                                                                                         typeof(short),
				                                                                                         typeof(ushort),
				                                                                                         typeof(int),
				                                                                                         typeof(uint),
				                                                                                         typeof(long),
				                                                                                         typeof(ulong),
				                                                                                         typeof(float),
				                                                                                         typeof(double),
				                                                                                         typeof(decimal
				                                                                                         ),
				                                                                                         typeof(
					                                                                                         DateTimeOffset
				                                                                                         ),
				                                                                                         typeof(DateTime
				                                                                                         ),
				                                                                                         typeof(string)
			                                                                                         }.YieldMetadata()));

		public static FrameworkCoercer Default { get; } = new FrameworkCoercer();

		FrameworkCoercer() : this(Supported) {}

		public FrameworkCoercer(ISpecification<TypeInfo> to) : base(to) {}

		protected override object Get(IConvertible parameter, TypeInfo targetType)
			=> Convert.ChangeType(parameter, targetType.AsType());
	}
}