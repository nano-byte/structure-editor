// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Windows.Forms;
using NanoByte.Common.Controls;
using NanoByte.Common.Undo;

namespace NanoByte.StructureEditor.WinForms;

/// <summary>
/// Edits a node in the structure using a <see cref="PropertyGrid"/>.
/// </summary>
/// <typeparam name="T">The type of element to edit.</typeparam>
public class PropertyGridNodeEditor<T> : NodeEditorBase<T> where T : class
{
    public PropertyGridNodeEditor()
    {
        SuspendLayout();
        SetupControls();
        ResumeLayout(performLayout: false);
    }

    private void SetupControls()
    {
        var propertyGrid = new ResettablePropertyGrid
        {
            ToolbarVisible = false,
            Dock = DockStyle.Fill
        };
        Controls.Add(propertyGrid);

        SetupUndoTracking(propertyGrid);
        TargetChanged += () => propertyGrid.SelectedObject = Target;
        OnRefresh += propertyGrid.Refresh;
    }

    private void SetupUndoTracking(PropertyGrid propertyGrid)
    {
        propertyGrid.PropertyValueChanged += (_, e) =>
        {
            if (Target != null) CommandExecutor?.Execute(new PropertyChangedCommand(Target, e));
        };
    }
}