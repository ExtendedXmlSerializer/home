using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class ImplicitTypingExtension : ISerializerExtension
	{
		readonly ImmutableArray<TypeInfo> _registered;

		public ImplicitTypingExtension(ImmutableArray<TypeInfo> registered) => _registered = registered;

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate<IIdentities>(Register)
			            .Decorate<ITypeIdentities>(Register);

		ITypeIdentities Register(IServiceProvider services, ITypeIdentities identities)
		{
			var formatter = services.Get<ITypeFormatter>();
			var entries   = new Types(formatter).Get(_registered);
			var result    = new ImplicitTypeIdentities(identities, entries);
			return result;
		}

		IIdentities Register(IServiceProvider services, IIdentities identities)
			=> new ImplicitIdentities(_registered, identities);

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class Types : IParameterizedSource<ImmutableArray<TypeInfo>, IParameterizedSource<string, TypeInfo>>
		{
			readonly ITypeFormatter       _formatter;
			readonly Func<TypeInfo, bool> _specification;

			public Types(ITypeFormatter formatter)
				: this(formatter, ApplicationTypeSpecification.Default.IsSatisfiedBy) {}

			public Types(ITypeFormatter formatter, Func<TypeInfo, bool> specification)
			{
				_formatter     = formatter;
				_specification = specification;
			}

			public IParameterizedSource<string, TypeInfo> Get(ImmutableArray<TypeInfo> parameter)
			{
				var items = parameter.Where(_specification)
				                     .Select(Item)
				                     .ToArray();
				var groups = items.GroupBy(x => x.Key)
				                  .ToArray();
				var invalid = groups.FirstOrDefault(x => x.Count() > 1);
				if (invalid != null)
				{
					var types = string.Join(", ", invalid.Select(x => x.Value.FullName));
					throw new InvalidOperationException(
					                                    $"An attempt was made to configure implicit types, but there is more than one type with the same name, which would result in conflicts.  Please remove one of these types or configure them to have different names from each other: {types} have the shared name {invalid.Key}.");
				}

				var store  = items.ToDictionary();
				var result = new TableSource<string, TypeInfo>(store);
				return result;
			}

			KeyValuePair<string, TypeInfo> Item(TypeInfo parameter)
				=> Pairs.Create(_formatter.Get(parameter), parameter);
		}
	}
}