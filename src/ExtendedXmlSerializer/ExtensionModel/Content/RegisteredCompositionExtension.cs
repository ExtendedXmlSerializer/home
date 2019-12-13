using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	interface ISerializerComposerResult
		: IParameterizedSource<IServices, ISerializerComposer>, IAlteration<IServiceRepository> {}

	interface ISerializerComposerResults : ITypedTable<ISerializerComposerResult>, ISerializerExtension {}

	sealed class ResultContainer : ITypedTable<ISerializerComposer>, ICommand<ITypedTable<ISerializerComposer>>
	{
		readonly ITypedTable<ISerializerComposer>[] _store;

		public ResultContainer() : this(new ITypedTable<ISerializerComposer>[1]) {}

		public ResultContainer(ITypedTable<ISerializerComposer>[] store) => _store = store;

		public bool IsSatisfiedBy(TypeInfo parameter) => _store[0].IsSatisfiedBy(parameter);

		public ISerializerComposer Get(TypeInfo parameter) => _store[0].Get(parameter);

		public void Assign(TypeInfo key, ISerializerComposer value)
		{
			_store[0].Assign(key, value);
		}

		public bool Remove(TypeInfo key) => _store[0].Remove(key);

		public void Execute(ITypedTable<ISerializerComposer> parameter)
		{
			_store[0] = parameter;
		}
	}

	sealed class RegisteredCompositionExtension : ISerializerExtension,
	                                              IAssignable<TypeInfo, ISerializerComposer>,
	                                              IAssignable<TypeInfo, Type>,
												  ICommand<TypeInfo>

	{
		readonly ITypedTable<ISerializerComposer>           _table;
		readonly ISerializerComposerResults                 _composers;

		[UsedImplicitly]
		public RegisteredCompositionExtension() : this(new ResultContainer()) {}

		public RegisteredCompositionExtension(ResultContainer container)
			: this(container, new SerializerComposerResults(container)) {}

		public RegisteredCompositionExtension(ITypedTable<ISerializerComposer> table,
		                                      ISerializerComposerResults composers)
		{
			_table     = table;
			_composers = composers;
		}

		public IServiceRepository Get(IServiceRepository parameter) => _composers.Get(parameter)
		                                                                         .RegisterInstance(_table)
		                                                                         .Decorate<IContents, Contents>();

		void ICommand<IServices>.Execute(IServices parameter)
		{
			_composers.Execute(parameter);
		}

		sealed class SerializerComposerResults : TypedTable<ISerializerComposerResult>, ISerializerComposerResults
		{
			readonly ICommand<ITypedTable<ISerializerComposer>>       _assign;
			readonly IDictionary<TypeInfo, ISerializerComposerResult> _store;

			public SerializerComposerResults(ICommand<ITypedTable<ISerializerComposer>> assign)
				: this(assign, new Dictionary<TypeInfo, ISerializerComposerResult>()) {}

			public SerializerComposerResults(ICommand<ITypedTable<ISerializerComposer>> assign,
			                                 IDictionary<TypeInfo, ISerializerComposerResult> store) : base(store)
			{
				_assign = assign;
				_store  = store;
			}

			public IServiceRepository Get(IServiceRepository parameter)
				=> _store.Values.Aggregate(parameter, (repository, pair) => pair.Get(repository));

			public void Execute(IServices parameter)
			{
				var table = new TypedTable<ISerializerComposer>();
				foreach (var result in _store)
				{
					table.Assign(result.Key, result.Value.Get(parameter));
				}

				_assign.Execute(table);
			}
		}

		sealed class FixedSerializerComposerResult
			: FixedInstanceSource<IServices, ISerializerComposer>, ISerializerComposerResult
		{
			public FixedSerializerComposerResult(ISerializerComposer instance) : base(instance) {}

			public IServiceRepository Get(IServiceRepository parameter) => parameter;
		}

		sealed class LocatedSerializerComposerResult : ISerializerComposerResult
		{
			readonly ISingletonLocator _locator;
			readonly Type              _composerType;

			public LocatedSerializerComposerResult(Type composerType) : this(SingletonLocator.Default, composerType) {}

			public LocatedSerializerComposerResult(ISingletonLocator locator, Type composerType)
			{
				_locator      = locator;
				_composerType = composerType;
			}

			public IServiceRepository Get(IServiceRepository parameter)
			{
				var singleton = _locator.Get(_composerType);
				var result = singleton != null
					             ? parameter.RegisterInstance(_composerType, singleton)
					             : parameter.Register(_composerType);
				return result;
			}

			public ISerializerComposer Get(IServices parameter) => parameter.GetService(_composerType)
			                                                                .AsValid<ISerializerComposer>();
		}

		sealed class Contents : IContents
		{
			readonly IContents                        _contents;
			readonly ITypedTable<ISerializerComposer> _table;

			public Contents(IContents contents, ITypedTable<ISerializerComposer> table)
			{
				_contents = contents;
				_table    = table;
			}

			public ContentModel.ISerializer Get(TypeInfo parameter)
			{
				var content = _contents.Get(parameter);
				var result = _table.IsSatisfiedBy(parameter)
					             ? _table.Get(parameter).Get(content)
					             : content;
				return result;
			}
		}

		public void Assign(TypeInfo key, ISerializerComposer value)
		{
			_composers.Assign(key, new FixedSerializerComposerResult(value));
		}

		public void Assign(TypeInfo key, Type value)
		{
			_composers.Assign(key, new LocatedSerializerComposerResult(value));
		}

		public void Execute(TypeInfo parameter)
		{
			_composers.Remove(parameter);
		}
	}
}