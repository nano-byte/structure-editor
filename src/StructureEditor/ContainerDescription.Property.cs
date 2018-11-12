// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NanoByte.Common;
using NanoByte.Common.Storage;
using NanoByte.Common.Undo;
using NanoByte.Common.Values;
using ICommandExecutor = NanoByte.Common.Undo.ICommandExecutor;

namespace NanoByte.StructureEditor
{
    public partial class ContainerDescription<TContainer>
    {
        /// <inheritdoc/>
        public IContainerDescription<TContainer> AddProperty<TProperty, TEditor>(string name, Func<TContainer, PropertyPointer<TProperty>> getPointer, TEditor editor)
            where TProperty : class, IEquatable<TProperty>, new()
            where TEditor : IEditorControl<TProperty>, new()
        {
            _descriptions.Add(new PropertyDescription<TProperty, TEditor>(name, getPointer));
            return this;
        }

        /// <inheritdoc/>
        public IContainerDescription<TContainer> AddPropertyContainerRef<TProperty, TEditor>(string name, Func<TContainer, PropertyPointer<TProperty>> getPointer, TEditor editor)
            where TProperty : class, IEquatable<TProperty>, new()
            where TEditor : IEditorControlContainerRef<TProperty, TContainer>, new()
        {
            _descriptions.Add(new PropertyDescriptionContainerRef<TProperty, TEditor>(name, getPointer));
            return this;
        }

        private class PropertyDescription<TProperty, TEditor> : DescriptionBase
            where TProperty : class, IEquatable<TProperty>, new()
            where TEditor : IEditorControl<TProperty>, new()
        {
            private readonly string _name;
            private readonly Func<TContainer, PropertyPointer<TProperty>> _getPointer;

            public PropertyDescription(string name, Func<TContainer, PropertyPointer<TProperty>> getPointer)
            {
                _name = name;
                _getPointer = getPointer;
            }

            public override IEnumerable<EntryInfo> GetEntriesIn(TContainer container)
            {
                var pointer = _getPointer(container);
                if (pointer.Value != null)
                {
                    var description = AttributeUtils.GetAttributes<DescriptionAttribute, TProperty>().FirstOrDefault();
                    yield return new EntryInfo(
                        name: _name,
                        description: description?.Description,
                        target: pointer.Value,
                        getEditorControl: executor => CreateEditor(container, pointer.Value, executor),
                        toXmlString: () => pointer.Value.ToXmlString(),
                        fromXmlString: xmlString =>
                        {
                            var newValue = XmlStorage.FromXmlString<TProperty>(xmlString);
                            return newValue.Equals(pointer.Value) ? null : SetValueCommand.For(pointer, newValue);
                        },
                        removeCommand: SetValueCommand.For(pointer, null));
                }
            }

            protected virtual TEditor CreateEditor(TContainer container, TProperty value, ICommandExecutor executor)
                => new TEditor {Target = value, CommandExecutor = executor};

            public override IEnumerable<ChildInfo> GetPossibleChildrenFor(TContainer container)
            {
                var description = AttributeUtils.GetAttributes<DescriptionAttribute, TProperty>().FirstOrDefault();
                return new[]
                {
                    new ChildInfo(
                        name: _name,
                        description: description?.Description,
                        create: () => SetValueCommand.For(_getPointer(container), new TProperty()))
                };
            }
        }

        private class PropertyDescriptionContainerRef<TProperty, TEditor> : PropertyDescription<TProperty, TEditor>
            where TProperty : class, IEquatable<TProperty>, new()
            where TEditor : IEditorControlContainerRef<TProperty, TContainer>, new()
        {
            public PropertyDescriptionContainerRef(string name, Func<TContainer, PropertyPointer<TProperty>> getPointer)
                : base(name, getPointer)
            {}

            protected override TEditor CreateEditor(TContainer container, TProperty value, ICommandExecutor executor)
                => new TEditor {Target = value, ContainerRef = container, CommandExecutor = executor};
        }
    }
}
