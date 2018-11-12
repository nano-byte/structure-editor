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
                where TEditor : IEditorControl<TElement>, new()
            {
                _descriptions.Add(new ElementDescription<TElement, TEditor>(name));
                return this;
            }

            /// <inheritdoc/>
            public IListDescription<TContainer, TList> AddElementContainerRef<TElement, TEditor>(string name, TElement element, TEditor editor)
                where TElement : class, TList, IEquatable<TElement>, new()
                where TEditor : IEditorControlContainerRef<TElement, TContainer>, new()
            {
                _descriptions.Add(new ElementDescriptionContainerRef<TElement, TEditor>(name));
                return this;
            }

            private interface IElementDescription
            {
                [CanBeNull]
                EntryInfo TryGetEntry(TContainer container, IList<TList> list, TList candidate);

                ChildInfo GetPossibleChildFor(IList<TList> list);
            }

            private class ElementDescription<TElement, TEditor> : IElementDescription
                where TElement : class, TList, IEquatable<TElement>, new()
                where TEditor : IEditorControl<TElement>, new()
            {
                private readonly string _name;

                public ElementDescription(string name) => _name = name;

                public EntryInfo TryGetEntry(TContainer container, IList<TList> list, TList candidate)
                {
                    if (!(candidate is TElement element)) return null;

                    var description = AttributeUtils.GetAttributes<DescriptionAttribute, TElement>().FirstOrDefault();
                    return new EntryInfo(
                        name: _name,
                        description: description?.Description,
                        target: element,
                        getEditorControl: executor => CreateEditor(container, element, executor),
                        toXmlString: () => element.ToXmlString(),
                        fromXmlString: xmlString =>
                        {
                            var newValue = XmlStorage.FromXmlString<TElement>(xmlString);
                            return newValue.Equals(element) ? null : new ReplaceInList<TList>(list, element, newValue);
                        },
                        removeCommand: new RemoveFromCollection<TList>(list, element));
                }

                protected virtual TEditor CreateEditor(TContainer container, TElement value, ICommandExecutor executor)
                    => new TEditor {Target = value, CommandExecutor = executor};

                public ChildInfo GetPossibleChildFor(IList<TList> list)
                {
                    var description = AttributeUtils.GetAttributes<DescriptionAttribute, TElement>().FirstOrDefault();
                    return new ChildInfo(
                        name: _name,
                        description: description?.Description,
                        create: () => new AddToCollection<TList>(list, new TElement()));
                }
            }

            private class ElementDescriptionContainerRef<TElement, TEditor> : ElementDescription<TElement, TEditor>
                where TElement : class, TList, IEquatable<TElement>, new()
                where TEditor : IEditorControlContainerRef<TElement, TContainer>, new()
            {
                public ElementDescriptionContainerRef(string name)
                    : base(name)
                {}

                protected override TEditor CreateEditor(TContainer container, TElement value, ICommandExecutor executor)
                    => new TEditor {Target = value, ContainerRef = container, CommandExecutor = executor};
            }
        }
    }
}
