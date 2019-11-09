using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	sealed class RegisteredCompositionExtension : ISerializerExtension, IAssignable<TypeInfo, ISerializerComposer>
	{
		readonly ITypedTable<ISerializerComposer> _composers;

		[UsedImplicitly]
		public RegisteredCompositionExtension() : this(new TypedTable<ISerializerComposer>()) {}

		public RegisteredCompositionExtension(ITypedTable<ISerializerComposer> composers) => _composers = composers;

		public IServiceRepository Get(IServiceRepository parameter) => parameter.RegisterInstance(_composers)
		                                                                        .Decorate<IContents, Contents>();

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class Contents : IContents
		{
			readonly IContents                        _contents;
			readonly ITypedTable<ISerializerComposer> _table;

			public Contents(IContents contents, ITypedTable<ISerializerComposer> table)
			{
				_contents = contents;
				_table    = table;
			}

			public ISerializer Get(TypeInfo parameter)
			{
				var content = _contents.Get(parameter);
				var result = _table.IsSatisfiedBy(parameter)
					             ? _table.Get(parameter)
					                     .Get(content)
					             : content;
				return result;
			}
		}

		public void Assign(TypeInfo key, ISerializerComposer value)
		{
			_composers.Assign(key, value);
		}
	}
}