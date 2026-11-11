// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.EtoControls;

namespace NanoByte.StructureEditor.EtoForms;

/// <summary>
/// Edits a node in the structure using a <see cref="PropertyEditor"/>.
/// </summary>
/// <typeparam name="T">The type of element to edit.</typeparam>
public class PropertyEditorNodeEditor<T> : NodeEditorBase<T> where T : class
{
    private readonly PropertyEditor _propertyEditor = new();

    /// <summary>
    /// Creates a new property editor node editor.
    /// </summary>
    public PropertyEditorNodeEditor()
    {
        _propertyEditor.PropertyValueChanged += (_, e) =>
        {
            // The property editor has already applied the new value
            if (Target != null) CommandExecutor?.Execute(e.ToPropertyChangedCommand(Target));
        };
        TargetChanged += () => _propertyEditor.SelectedObject = Target;
        OnRefresh += _propertyEditor.RefreshValues;
        OnCommitPendingChanges += _propertyEditor.CommitPendingChanges;

        Content = _propertyEditor;
    }
}
