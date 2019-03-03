// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Windows.Forms;
using NanoByte.Common.Controls;
using NanoByte.Common.Native;
using NanoByte.Common.Undo;

namespace NanoByte.StructureEditor.WinForms
{
    /// <summary>
    /// Edits arbitrary types of elements using a <see cref="PropertyGrid"/>. Provides optional <see cref="Common.Undo"/> support.
    /// </summary>
    /// <typeparam name="T">The type of element to edit.</typeparam>
    public class PropertyGridEditor<T> : EditorControlBase<T> where T : class
    {
        public PropertyGridEditor()
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
            if (UnixUtils.IsUnix)
            { // WORKAROUND: e.OldValue is not reliable on Mono, use MultiPropertyTracker instead
                var tracker = new MultiPropertyTracker(propertyGrid);
                propertyGrid.PropertyValueChanged += (sender, e) => CommandExecutor?.Execute(tracker.GetCommand(e.ChangedItem));
            }
            else
                propertyGrid.PropertyValueChanged += (sender, e) => CommandExecutor?.Execute(new PropertyChangedCommand(Target, e));
        }
    }
}
