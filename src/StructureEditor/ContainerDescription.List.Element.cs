// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;
using NanoByte.Common.Storage;
using NanoByte.Common.Undo;
using NanoByte.Common.Values;
using ICommandExecutor = NanoByte.Common.Undo.ICommandExecutor;

namespace NanoByte.StructureEditor
{
    public partial class ContainerDescription<TContainer>
    {
        private partial class ListDescription<TList>
        {
            /// <inheritdoc/>
            public IListDescription<TContainer, TList> AddElement<TElement, TEditor>(string name, TElement element, TEditor editor)
                where TElement : class, TList, IEquatable<TElement>, new()
                where TEditor : INodeEditor<TElement>, new()
            {
                _descriptions.Add(new ElementDescription<TElement, TEditor>(name));
                return this;
            }

            private interface IElementDescription
            {
                [CanBeNull]
                Node TryGetNode(TContainer container, IList<TList> list, TList candidate);

                NodeCandidate GetCandidatesFor(IList<TList> list);
            }

            private class ElementDescription<TElement, TEditor> : IElementDescription
                where TElement : class, TList, IEquatable<TElement>, new()
                where TEditor : INodeEditor<TElement>, new()
            {
                private readonly string _name;

                public ElementDescription(string name) => _name = name;

                public Node TryGetNode(TContainer container, IList<TList> list, TList candidate)
                {
                    if (!(candidate is TElement element)) return null;

                    var description = AttributeUtils.GetAttributes<DescriptionAttribute, TElement>().FirstOrDefault();
                    return new Node(
                        name: _name,
                        description: description?.Description,
                        target: element,
                        getEditorControl: executor => CreateEditor<TEditor, TElement>(container, element, executor),
                        getSerialized: () => element.ToXmlString(),
                        getUpdateCommand: value =>
                        {
                            var newValue = XmlStorage.FromXmlString<TElement>(value);
                            return newValue.Equals(element) ? null : new ReplaceInList<TList>(list, element, newValue);
                        },
                        removeCommand: new RemoveFromCollection<TList>(list, element));
                }

                public NodeCandidate GetCandidatesFor(IList<TList> list)
                {
                    var description = AttributeUtils.GetAttributes<DescriptionAttribute, TElement>().FirstOrDefault();
                    return new NodeCandidate(
                        name: _name,
                        description: description?.Description,
                        getCreateCommand: () => new AddToCollection<TList>(list, new TElement()));
                }
            }
        }
    }
}
