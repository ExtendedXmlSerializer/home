using System;
using System.Collections;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.ProcessModel;
using ExtendedXmlSerialization.ProcessModel.Write;
using ExtendedXmlSerialization.Services;

namespace ExtendedXmlSerialization.Instructions.Write
{
    class EmitInstanceMembersInstruction : WriteInstructionBase
    {
        public static EmitInstanceMembersInstruction Default { get; } = new EmitInstanceMembersInstruction();
        EmitInstanceMembersInstruction() : this(MemberTemplates.Default) {}

        private readonly IMemberTemplates _templates;

        public EmitInstanceMembersInstruction(IMemberTemplates templates)
        {
            _templates = templates;
        }

        protected override void OnExecute(ISerialization services)
        {
            //var factory = services.GetValid<IScopeFactory>();
            var templates = _templates.Get(services.Current.Definition.Type);
            using (var scope = services.Create(templates.Source))
            {
                services.Execute(scope);

                var items = templates.Templates;
                var count = items.Count;
                for (var i = 0; i < count; i++)
                {
                    var item = items[i];
                    using (var child = scope.CreateScope(item))
                    {
                        services.Execute(child);
                    }
                }
            }
        }
    }

    /*abstract class EmitInstanceInstructionBase : WriteInstructionBase
    {
        private readonly IElementInformation _information;
        private readonly IInstruction _body;

        protected EmitInstanceInstructionBase(IElementInformation information, IInstruction body)
        {
            _information = information;
            _body = body;
        }

        protected abstract object GetInstance(IContext context);

        protected override void OnExecute(ISerialization services)
        {
            var instance = GetInstance(services.Current);
            using (services.New(instance, _information))
            {
                _body.Execute(services);
            }
        }
    }*/

    class EmitDictionaryInstruction : WriteInstructionBase<IDictionary>
    {
        public static EmitDictionaryInstruction Default { get; } = new EmitDictionaryInstruction();

        EmitDictionaryInstruction()
            : this(EmitInstanceMembersInstruction.Default/*, EmitInstanceBodyInstructionInstance.Default*/) {}

        private readonly IInstruction _members;
        private readonly IInstruction _body;

        public EmitDictionaryInstruction(IInstruction members/*, IInstruction body*/)
        {
            _members = members;
            /*_body = body;*/
        }

        protected override void Execute(ISerialization services, IDictionary instance)
        {
            _members.Execute(services);

            foreach (DictionaryEntry item in instance)
            {
                using (var scope = services.CreateScope(item))
                {
                    /*using (services.New(item.Key, DictionaryKeyElementInformation.Default))
                    {
                        _body.Execute(services);
                    }

                    using (services.New(item.Value, DictionaryValueElementInformation.Default))
                    {
                        _body.Execute(services);
                    }*/
                }
            }
        }
    }

    class EmitEnumerableInstruction : WriteInstructionBase
    {
        public static EmitEnumerableInstruction Default { get; } = new EmitEnumerableInstruction();

        EmitEnumerableInstruction()
            : this(
                EmitInstanceMembersInstruction.Default/*,
                EmitInstanceBodyInstructionInstance.Default, ItemElements.DefaultElement.Get*/) {}

        private readonly IInstruction _members;
        /*private readonly IInstruction _body;
        private readonly Func<ITypeDefinition, IElementInformation> _elements;*/

        public EmitEnumerableInstruction(IInstruction members/*, IInstruction body,
                                         Func<ITypeDefinition, IElementInformation> elements*/)
        {
            _members = members;
            /*_body = body;
            _elements = elements;*/
        }

        protected override void OnExecute(ISerialization services)
        {
            _members.Execute(services);

            var array = Arrays.Default.AsArray(services.Current.Instance);
            var length = array.Length;
            for (var i = 0; i < length; i++)
            {
                using (var scope = services.Create(array.GetValue(i)))
                {
                    services.Execute(scope);
                    // _body.Execute(services);
                }
            }
        }
    }

    class EmitMemberInstruction : WriteInstructionBase
    {
        public static EmitMemberInstruction Default { get; } = new EmitMemberInstruction();
        EmitMemberInstruction() {}

        protected override void OnExecute(ISerialization services)
        {
            // TODO: Implemenent.
            // services.Create()
        }
    }

    /*class EmitMemberInstruction : EmitInstanceInstruction
    {
        public static EmitMemberInstruction Default { get; } = new EmitMemberInstruction();

        EmitMemberInstruction()
            : base(context => context.Value(), MemberElementInformation.Instance,
                   EmitInstanceBodyInstructionInstance.Default) {}
    }

    class EmitInstanceInstruction : EmitInstanceInstructionBase
    {
        private readonly Func<IContext, object> _instance;

        public EmitInstanceInstruction(Func<IContext, object> instance, IElementInformation information,
                                       IInstruction body) : base(information, body)
        {
            _instance = instance;
        }

        protected override object GetInstance(IContext context) => _instance(context);
    }*/

    /*class DefaultEmitInstanceMembersInstruction : EmitInstanceMembersInstruction
    {
        public new static DefaultEmitInstanceMembersInstruction Default { get; } =
            new DefaultEmitInstanceMembersInstruction();

        DefaultEmitInstanceMembersInstruction()
            : base(DefaultEmitTypeForInstanceInstruction.Default, MemberTemplates.Default) {}
    }*/
}