// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Linq;
using System.Windows.Forms;
using JetBrains.Annotations;
using NanoByte.Common;
using NanoByte.Common.Collections;
using NanoByte.Common.Dispatch;
using NanoByte.Common.Undo;
using NanoByte.StructureEditor.WinForms.Resources;

namespace NanoByte.StructureEditor.WinForms
{
    /// <summary>
    /// An editor for hierarchical structures.
    /// </summary>
    /// <remarks>Derive and call <see cref="DescribeRoot"/> or <see cref="DescribeRoot{TEditor}"/> as well as <see cref="Describe{TContainer}"/> in the constructor.</remarks>
    /// <typeparam name="T">The type of object to edit.</typeparam>
    [PublicAPI]
    public class StructureEditor<T> : UserControl, IStructureEditor<T>
        where T : class, IEquatable<T>, new()
    {
        #region Controls
        private readonly ToolStripDropDownButton _buttonAdd = new ToolStripDropDownButton
        {
            Text = "Add",
            Image = Images.AddButton
        };

        private readonly ToolStripButton _buttonRemove = new ToolStripButton
        {
            DisplayStyle = ToolStripItemDisplayStyle.Image,
            Text = "Remove",
            Image = Images.DeleteButton
        };

        private readonly TreeView _treeView = new TreeView
        {
            Dock = DockStyle.Fill,
            ShowNodeToolTips = true
        };

        private readonly Panel _editorPanel = new Panel
        {
            Dock = DockStyle.Fill
        };

        private readonly ValidatingTextEditor _textEditor = new ValidatingTextEditor
        {
            Dock = DockStyle.Fill
        };

        public StructureEditor()
        {
            SuspendLayout();
            SetupControls();
            ResumeLayout(performLayout: false);
        }

        private void SetupControls()
        {
            _buttonAdd.DropDownOpening += buttonAdd_DropDownOpening;
            _buttonRemove.Click += buttonRemove_Click;
            _treeView.AfterSelect += treeView_AfterSelect;
            _textEditor.ContentChanged += TextEditorContentChanged;

            Controls.Add(new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterWidth = 9,
                SplitterDistance = 100,
                Panel1 =
                {
                    Controls =
                    {
                        new SplitContainer
                        {
                            Dock = DockStyle.Fill,
                            Orientation = Orientation.Vertical,
                            SplitterWidth = 9,
                            SplitterDistance = 80,
                            Panel1 =
                            {
                                Controls =
                                {
                                    _treeView,
                                    new ToolStrip
                                    {
                                        GripStyle = ToolStripGripStyle.Hidden,
                                        Items = {_buttonAdd, new ToolStripSeparator(), _buttonRemove}
                                    }
                                }
                            },
                            Panel2 = {Controls = {_editorPanel}}
                        }
                    }
                },
                Panel2 = {Controls = {_textEditor}}
            });
        }
        #endregion

        //--------------------//

        #region Describe
        [ItemNotNull]
        private readonly AggregateDispatcher<object, EntryInfo> _getEntries = new AggregateDispatcher<object, EntryInfo>();

        [ItemCanBeNull]
        private readonly AggregateDispatcher<object, ChildInfo> _getPossibleChildren = new AggregateDispatcher<object, ChildInfo>();

        /// <summary>
        /// Adds a <see cref="ContainerDescription{TContainer}"/> used to describe the structure of the data being editing.
        /// </summary>
        /// <typeparam name="TContainer">The type of the container to describe.</typeparam>
        /// <returns>The <see cref="ContainerDescription{TContainer}"/> for use in a "Fluent API" style.</returns>
        public IContainerDescription<TContainer> Describe<TContainer>()
            where TContainer : class
        {
            var description = new ContainerDescription<TContainer>();
            _getEntries.Add<TContainer>(container => description.GetEntriesIn(container).ToList());
            _getPossibleChildren.Add<TContainer>(container => description.GetPossibleChildrenFor(container).Append(null).ToList());
            return description;
        }

        /// <summary>
        /// Set up handling for the root element with a custom editor.
        /// </summary>
        /// <typeparam name="TEditor">An editor for modifying the content of the root.</typeparam>
        /// <param name="name">The name of the root element.</param>
        /// <returns>The <see cref="ContainerDescription{TContainer}"/> for use in a "Fluent API" style.</returns>
        public IContainerDescription<T> DescribeRoot<TEditor>(string name)
            where TEditor : IEditorControl<T>, new()
        {
            // Use CommandManager as root rather than Target, to allow the entire Target to be replaced during editing
            Describe<ICommandManager<T>>()
               .AddProperty(name, x => PropertyPointer.For(() => CommandManager.Target, value => CommandManager.Target = value), new TEditor());

            return Describe<T>();
        }

        /// <summary>
        /// Set up handling for the root element with a generic editor.
        /// </summary>
        /// <param name="name">The name of the root element.</param>
        /// <returns>The <see cref="ContainerDescription{TContainer}"/> for use in a "Fluent API" style.</returns>
        public IContainerDescription<T> DescribeRoot(string name)
            => DescribeRoot<PropertyGridEditor<T>>(name);
        #endregion

        #region Target
        /// <summary>
        /// Holds the object being editing and manages undo/redo operations on it.
        /// </summary>
        public ICommandManager<T> CommandManager { get; private set; } = new CommandManager<T>(new T());

        /// <summary>
        /// Opens an object for editing using the specified <see cref="ICommandManager{T}"/>.
        /// </summary>
        /// <param name="commandManager">Holds the object being editing and manages undo/redo operations on it.</param>
        public void Open([NotNull] ICommandManager<T> commandManager)
        {
            if (commandManager == null) throw new ArgumentNullException(nameof(commandManager));

            if (CommandManager != null) CommandManager.TargetUpdated -= RebuildOnNextIdle;
            CommandManager = commandManager;
            CommandManager.TargetUpdated += RebuildOnNextIdle;

            RebuildTree();

            void RebuildOnNextIdle()
            {
                Application.Idle += RebuildOnce;

                void RebuildOnce(object sender, EventArgs e)
                {
                    RebuildTree();
                    Application.Idle -= RebuildOnce;
                }
            }
        }
        #endregion

        #region Build nodes
        /// <summary>
        /// Rebuilds the <see cref="_treeView"/> node while attempting to retain the current selection.
        /// </summary>
        private void RebuildTree()
        {
            Node reselectNode = null;

            _treeView.BeginUpdate();
            _treeView.Nodes.Clear();
            _treeView.Nodes.AddRange(BuildNodes(CommandManager));
            _treeView.SelectedNode = reselectNode ?? _treeView.Nodes.Cast<Node>().FirstOrDefault();
            _treeView.SelectedNode?.Expand();
            _treeView.EndUpdate();

            TreeNode[] BuildNodes(object target)
            {
                return _getEntries.Dispatch(target).Select(entry =>
                {
                    var node = new Node(entry, BuildNodes(entry.Target));
                    if (entry.Target == _selectedTarget) reselectNode = node;
                    return (TreeNode)node;
                }).ToArray();
            }
        }
        #endregion

        #region Undo
        /// <summary>
        /// Undoes the last action.
        /// </summary>
        public void Undo()
        {
            if (_textEditor.TextEditor.EnableUndo) _textEditor.TextEditor.Undo();
            else CommandManager.Undo();
        }

        /// <summary>
        /// Redoes the last action.
        /// </summary>
        public void Redo()
        {
            if (_textEditor.TextEditor.EnableRedo) _textEditor.TextEditor.Redo();
            else CommandManager.Redo();
        }
        #endregion

        //--------------------//

        #region Add/remove
        private void buttonAdd_DropDownOpening(object sender, EventArgs e)
        {
            _buttonAdd.DropDownItems.Clear();
            if (SelectedNode != null)
                BuildAddDropDownMenu(SelectedNode.Entry.Target);
        }

        private void BuildAddDropDownMenu(object instance)
        {
            foreach (var child in _getPossibleChildren.Dispatch(instance))
            {
                if (child == null) _buttonAdd.DropDownItems.Add(new ToolStripSeparator());
                else
                {
                    var child1 = child;
                    _buttonAdd.DropDownItems.Add(new ToolStripMenuItem(child.Name, null, delegate
                        {
                            var command = child1.Create();
                            _selectedTarget = command.Value;
                            CommandManager.Execute(command);
                        })
                        {ToolTipText = child.Description});
                }
            }
        }

        /// <summary>
        /// Removes the currently selected entry;
        /// </summary>
        public void Remove()
        {
            if (SelectedNode == null || _treeView.SelectedNode == _treeView.Nodes[0]) return;

            var deleteCommand = SelectedNode.Entry.RemoveCommand;
            _treeView.SelectedNode = _treeView.SelectedNode.Parent; // Select parent before deleting
            CommandManager.Execute(deleteCommand);
        }

        private void buttonRemove_Click(object sender, EventArgs e) => Remove();
        #endregion

        #region Selection
        private object _selectedTarget;
        private object _editingTarget;
        private object _xmlTarget;

        private Node SelectedNode => (Node)_treeView.SelectedNode;

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _buttonRemove.Enabled = _treeView.Nodes.Count > 0 && e.Node != _treeView.Nodes[0];
            _selectedTarget = SelectedNode.Entry.Target;

            if (_selectedTarget == _editingTarget) _editorControl.Refresh();
            else
            {
                UpdateEditorControl();
                _editingTarget = _selectedTarget;
            }

            if (_selectedTarget != _xmlTarget) _textEditor.SetContent(ToXmlString(), "XML");
            _xmlTarget = null;
        }

        private Control _editorControl;

        private void UpdateEditorControl()
        {
            if (_editorControl != null)
            {
                _editorPanel.Controls.Remove(_editorControl);
                _editorControl.Dispose();
            }

            _editorControl = (Control)SelectedNode.Entry.GetEditorControl(CommandManager);
            _editorControl.Dock = DockStyle.Fill;
            _editorPanel.Controls.Add(_editorControl);
        }

        /// <summary>
        /// Returns the XML representation of the <see cref="SelectedNode"/>.
        /// </summary>
        protected virtual string ToXmlString() => SelectedNode.Entry.ToXmlString()
            // Hide <?xml> header
            .GetRightPartAtFirstOccurrence('\n');

        private void TextEditorContentChanged(string text)
        {
            var command = SelectedNode.Entry.FromXmlString(text);
            if (command == null) return;
            _xmlTarget = _selectedTarget = command.Value;
            CommandManager.Execute(command);
            _textEditor.TextEditor.Document.UndoStack.ClearAll();
        }
        #endregion
    }
}
