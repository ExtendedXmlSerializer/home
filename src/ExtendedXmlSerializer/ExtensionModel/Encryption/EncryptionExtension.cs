using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Content.Members;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ExtensionModel.Encryption
{
	public sealed class EncryptionExtension : ISerializerExtension, ISpecification<IAlteration<IConverter>>
	{
		readonly static EncryptionConverterAlteration Alteration = EncryptionConverterAlteration.Default;

		readonly ISpecification<MemberInfo> _specification;
		readonly IAlteration<IConverter>    _alteration;

		[UsedImplicitly]
		public EncryptionExtension() : this(Alteration) {}

		public EncryptionExtension(IAlteration<IConverter> alteration) : this(alteration, new HashSet<MemberInfo>()) {}

		public EncryptionExtension(ICollection<MemberInfo> registered) : this(Alteration, registered) {}

		public EncryptionExtension(IAlteration<IConverter> alteration, ICollection<MemberInfo> registered)
			: this(new ContainsSpecification<MemberInfo>(registered), alteration, registered) {}

		public EncryptionExtension(ISpecification<MemberInfo> specification, IAlteration<IConverter> alteration,
		                           ICollection<MemberInfo> registered)
		{
			_specification = specification;
			_alteration    = alteration;
			Registered     = registered;
		}

		public bool IsSatisfiedBy(IAlteration<IConverter> parameter) => parameter == _alteration;

		public ICollection<MemberInfo> Registered { get; }

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate<IMemberContents>(Register)
			            .Decorate<IMemberConverters>(Register);

		IMemberConverters Register(IServiceProvider services, IMemberConverters converters)
			=> new AlteredMemberConverters(_specification, _alteration, converters);

		IMemberContents Register(IServiceProvider services, IMemberContents contents)
			=> new AlteredMemberContents(_specification, _alteration, contents, services.Get<IConverters>(),
			                             services.Get<ISerializers>()
			                                     .Get(Support<string>.Key));

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}