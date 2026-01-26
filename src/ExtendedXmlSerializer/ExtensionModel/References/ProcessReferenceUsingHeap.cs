using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.References;

sealed class ProcessReferenceUsingHeap : ICommand<ProcessReferenceInput>
{
    readonly ISpecification<TypeInfo> _allowed;
    readonly ITypeMembers             _members;
    readonly IMemberAccessors         _accessors;
    readonly IEnumeratorStore         _store;

    // ReSharper disable once TooManyDependencies
    public ProcessReferenceUsingHeap(IContainsCustomSerialization custom, ITypeMembers members, IMemberAccessors accessors,
                            IEnumeratorStore store)
        : this(AssignedSpecification<TypeInfo>.Default.And(custom.Inverse()), members, accessors, store) { }

    // ReSharper disable once TooManyDependencies
    public ProcessReferenceUsingHeap(ISpecification<TypeInfo> allowed, ITypeMembers members, IMemberAccessors accessors,
                            IEnumeratorStore store)
    {
        _allowed = allowed;
        _members = members;
        _accessors = accessors;
        _store = store;
    }

    public void Execute(ProcessReferenceInput parameter)
    {
        var (results, head) = parameter;
        results.IsSatisfiedBy(head);

        var stack = new Stack<(object Node, ReferenceBoundary Boundary)>();
        stack.Push((head, results.Get(head)));
        try
        {
            while (stack.Count > 0)
            {
                var poped = stack.Pop();
                var node = poped.Node;
                using var boundary = poped.Boundary;

                var type = node.GetType();
                if (!_allowed.IsSatisfiedBy(type))
                    continue;

                var members = _members.Get(type);
                var children = new List<object>(members.Length);
                for (var i = 0; i < members.Length; i++)
                {
                    var value = _accessors.Get(members[i]).Get(node);
                    if (results.IsSatisfiedBy(value))
                        children.Add(value);
                }

                var iterator = _store.For(node);
                while (iterator?.MoveNext() ?? false)
                {
                    var current = iterator.Current;
                    if (results.IsSatisfiedBy(current))
                        children.Add(current);
                }

                for (var i = children.Count - 1; i >= 0; i--)
                {
                    stack.Push((children[i], results.Get(children[i])));
                }
            }
        }
        finally
        {
            while (stack.Count > 0)
            {
                stack.Pop().Boundary.Dispose();
            }
        }

    }
}