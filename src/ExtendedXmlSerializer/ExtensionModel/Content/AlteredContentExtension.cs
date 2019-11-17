using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	public class AlteredContentExtension : ISerializerExtension
	{
		[UsedImplicitly]
		public AlteredContentExtension() :
			this(new TableSource<TypeInfo, ContentAlteration>(ReflectionModel.Defaults.TypeComparer),
			     new TableSource<MemberInfo, ContentAlteration>(MemberComparer.Default)) {}

		public AlteredContentExtension(ITableSource<TypeInfo, ContentAlteration> types,
		                               ITableSource<MemberInfo, ContentAlteration> members)
		{
			Types   = types;
			Members = members;
		}

		public ITableSource<TypeInfo, ContentAlteration> Types { get; }
		public ITableSource<MemberInfo, ContentAlteration> Members { get; }

		public IServiceRepository Get(IServiceRepository parameter) => parameter.RegisterInstance(Types)
		                                                                        .RegisterInstance(Members)
		                                                                        .Decorate<IContents, Contents>()
		                                                                        .Decorate<IMemberContents,
			                                                                        MemberContents>();

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class Contents : IContents
		{
			readonly ITableSource<TypeInfo, ContentAlteration> _registrations;
			readonly IContents                                 _contents;

			public Contents(ITableSource<TypeInfo, ContentAlteration> registrations, IContents contents)
			{
				_registrations = registrations;
				_contents      = contents;
			}

			public ContentModel.ISerializer Get(TypeInfo parameter)
			{
				var serializer = _contents.Get(parameter);
				var result = _registrations.IsSatisfiedBy(parameter)
					             ? new Serializer(_registrations.Get(parameter), serializer)
					             : serializer;
				return result;
			}
		}

		sealed class MemberContents : IMemberContents
		{
			readonly ITableSource<MemberInfo, ContentAlteration> _registrations;
			readonly IMemberContents                             _contents;

			public MemberContents(ITableSource<MemberInfo, ContentAlteration> registrations, IMemberContents contents)
			{
				_registrations = registrations;
				_contents      = contents;
			}

			public ContentModel.ISerializer Get(IMember parameter)
			{
				var serializer = _contents.Get(parameter);
				var result = _registrations.IsSatisfiedBy(parameter.Metadata)
					             ? new Serializer(_registrations.Get(parameter.Metadata), serializer)
					             : serializer;
				return result;
			}
		}

		sealed class Serializer : ContentModel.ISerializer
		{
			readonly IAlteration<object> _read;
			readonly IAlteration<object> _write;
			readonly ContentModel.ISerializer         _serializer;

			public Serializer(ContentAlteration alteration, ContentModel.ISerializer serializer) : this(alteration.Read,
			                                                                               alteration.Write,
			                                                                               serializer) {}

			public Serializer(IAlteration<object> read, IAlteration<object> write, ContentModel.ISerializer serializer)
			{
				_read       = read;
				_write      = write;
				_serializer = serializer;
			}

			public object Get(IFormatReader parameter) => _read.Get(_serializer.Get(parameter));

			public void Write(IFormatWriter writer, object instance)
			{
				_serializer.Write(writer, _write.Get(instance));
			}
		}
	}
}