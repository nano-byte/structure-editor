// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NanoByte.Common.Undo;
using NanoByte.StructureEditor.WinForms.Resources;
using System.ComponentModel;

namespace NanoByte.StructureEditor.WinForms;

/// <summary>
/// An editor for hierarchical structures.
/// </summary>
/// <remarks>Derive and call <see cref="DescribeRoot"/> or <see cref="DescribeRoot{TEditor}"/> as well as <see cref="Describe{TContainer}"/> in the constructor.</remarks>
/// <typeparam name="T">The type of object to edit.</typeparam>
public class StructureEditor<T> : UserControl, IStructureEditor<T>
    where T : class
{
    private readonly StructureEditorPresenter<T> _presenter;

    /// <summary>
    /// Creates a new structure editor.
    /// </summary>
    /// <param name="factory">Callback to create a new instance of <typeparamref name="T"/>.</param>
    public StructureEditor(Func<T> factory)
    {
        _presenter = new(factory);
        _presenter.StructureChanged += RebuildOnNextIdle;

        SuspendLayout();
        SetupControls();
        ResumeLayout(performLayout: false);
    }

    /// <summary>
    /// Creates a new structure editor for a <typeparamref name="T"/> with a parameterless constructor.
    /// </summary>
    public StructureEditor() : this(Activator.CreateInstance<T>) {}

    #region Controls
    private readonly ToolStripDropDownButton _buttonAdd = new()
    {
        Text = "&Add",
        Image = Images.AddButton
    };

    private readonly ToolStripButton _buttonRemove = new()
    {
        Text = "&Remove",
        Image = Images.DeleteButton
    };

    private readonly TreeView _treeView = new()
    {
        Dock = DockStyle.Fill,
        ShowNodeToolTips = true
    };

    private readonly Panel _editorPanel = new()
    {
        Dock = DockStyle.Fill
    };

    private readonly ValidatingTextEditor _textEditor = new()
    {
        Dock = DockStyle.Fill
    };

    private void SetupControls()
    {
        _buttonRemove.Click += (_, _) => Remove();
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
    /// <inheritdoc/>
    public IContainerDescription<TContainer> Describe<TContainer>()
        where TContainer : class
        => _presenter.Describe<TContainer>();

    /// <inheritdoc/>
    public IContainerDescription<T> DescribeRoot<TEditor>(string name)
        where TEditor : INodeEditor<T>, new()
        => _presenter.DescribeRoot<TEditor>(name);

    /// <inheritdoc/>
    public IContainerDescription<T> DescribeRoot(string name)
        => DescribeRoot<PropertyGridNodeEditor<T>>(name);
    #endregion

    #region Target
    /// <summary>
    /// Holds the object being edited and manages undo/redo operations on it.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ICommandManager<T> CommandManager => _presenter.CommandManager;

    /// <summary>
    /// The serializer used for the text representation of the selected node. Defaults to <see cref="XmlStructureSerializer"/>.
    /// </summary>
    /// <remarks>Set automatically by <see cref="Open"/> when passed a <see cref="SerializingCommandManager{T}"/>.</remarks>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IStructureSerializer Serializer
    {
        get => _presenter.Serializer;
        set
        {
            _presenter.Serializer = value;
            _textEditor.SetContent(GetSerialized(), value);
        }
    }

    /// <summary>
    /// Opens an object for editing using the specified <see cref="ICommandManager{T}"/>.
    /// </summary>
    /// <param name="commandManager">Holds the object being edited and manages undo/redo operations on it. If this is a <see cref="SerializingCommandManager{T}"/> its <see cref="SerializingCommandManager{T}.Serializer"/> is also applied to <see cref="Serializer"/>.</param>
    public void Open(ICommandManager<T> commandManager)
    {
        if (commandManager == null) throw new ArgumentNullException(nameof(commandManager));

        // Cancel any rebuild that was scheduled against the previous CommandManager
        Application.Idle -= RebuildOnce;
        _rebuildScheduled = false;

        _presenter.Open(commandManager);

        RebuildTree();
        if (commandManager is SerializingCommandManager<T>) _textEditor.SetContent(GetSerialized(), Serializer);
    }

    private bool _rebuildScheduled;

    private void RebuildOnNextIdle()
    {
        if (_rebuildScheduled) return;
        _rebuildScheduled = true;
        Application.Idle += RebuildOnce;
    }

    private void RebuildOnce(object? sender, EventArgs e)
    {
        Application.Idle -= RebuildOnce;
        _rebuildScheduled = false;
        RebuildTree();
    }
    #endregion

    #region Dispose
    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        try
        {
            if (disposing)
            {
                _presenter.StructureChanged -= RebuildOnNextIdle;
                _presenter.Close();
                Application.Idle -= RebuildOnce;
            }
        }
        finally
        {
            base.Dispose(disposing);
        }
    }
    #endregion

    #region Build nodes
    /// <summary>
    /// Rebuilds the <see cref="_treeView"/> node while attempting to retain the current selection and expansion state.
    /// </summary>
    private void RebuildTree()
    {
        var expandedTargets = GetExpandedTargets(_treeView.Nodes);
        var reexpandNodes = new List<TreeNode>();
        TreeNode? reselectNode = null;

        TreeNode[] GetTreeNodes(IEnumerable<Node> nodes)
            => nodes.Select(TreeNode (node) =>
            {
                var treeNode = new StructureTreeNode(node, GetTreeNodes(_presenter.GetNodes(node.Target)));
                if (_presenter.ShouldSelect(node)) reselectNode = treeNode;
                if (node.Target != null && expandedTargets.Contains(node.Target)) reexpandNodes.Add(treeNode);
                return treeNode;
            }).ToArray();

        _treeView.BeginUpdate();
        _treeView.Nodes.Clear();
        _treeView.Nodes.AddRange(GetTreeNodes(_presenter.GetRootNodes()));
        foreach (var treeNode in reexpandNodes) treeNode.Expand();
        // ReSharper disable once ConstantNullCoalescingCondition
        _treeView.SelectedNode = reselectNode ?? _treeView.Nodes.Cast<TreeNode>().FirstOrDefault();
        _treeView.SelectedNode?.Expand();
        _treeView.EndUpdate();
    }

    /// <summary>
    /// Collects the <see cref="Node.Target"/>s of all currently expanded tree nodes.
    /// </summary>
    private static HashSet<object> GetExpandedTargets(TreeNodeCollection nodes)
    {
        var targets = new HashSet<object>(StructureEditorPresenter<T>.TargetComparer);

        void Collect(TreeNodeCollection collection)
        {
            foreach (TreeNode treeNode in collection)
            {
                if (treeNode.IsExpanded && treeNode is StructureTreeNode {Node.Target: {} target}) targets.Add(target);
                Collect(treeNode.Nodes);
            }
        }

        Collect(nodes);
        return targets;
    }
    #endregion

    #region Undo
    /// <summary>
    /// Undoes the last action.
    /// </summary>
    public void Undo()
    {
        if (_textEditor.EnableUndo) _textEditor.Undo();
        else CommandManager.Undo();
    }

    /// <summary>
    /// Redoes the last action.
    /// </summary>
    public void Redo()
    {
        if (_textEditor.EnableRedo) _textEditor.Redo();
        else CommandManager.Redo();
    }
    #endregion

    //--------------------//

    #region Add/remove
    private void BuildAddDropDownMenu()
    {
        ToolStripItem MenuItem(NodeCandidate? candidate)
            => candidate == null
                ? new ToolStripSeparator()
                : new ToolStripMenuItem(candidate.NodeType, null, (_, _) => _presenter.Add(candidate)) {ToolTipText = candidate.Description};

        var menu = _presenter.GetCandidates().Select(MenuItem).ToArray();

        _buttonAdd.DropDownItems.Clear();
        _buttonAdd.DropDownItems.AddRange(menu);

        _buttonAdd.Enabled = menu.Length != 0;
    }

    /// <summary>
    /// Removes the currently selected entry.
    /// </summary>
    public void Remove()
    {
        if (SelectedNode == null || _treeView.SelectedNode == _treeView.Nodes[0]) return;

        var removeCommand = _presenter.GetRemoveCommand();
        if (removeCommand == null) return;

        _treeView.SelectedNode = _treeView.SelectedNode?.Parent; // Select parent before deleting
        CommandManager.Execute(removeCommand);
    }
    #endregion

    #region Selection
    /// <summary>
    /// The target object of the currently selected node; <c>null</c> if nothing is selected.
    /// </summary>
    public object? SelectedTarget => SelectedNode?.Node.Target;

    /// <summary>
    /// Raised when the selected node in the tree changes.
    /// </summary>
    public event Action? SelectionChanged;

    private StructureTreeNode? SelectedNode => _treeView.SelectedNode as StructureTreeNode;

    private void treeView_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        _presenter.Select(SelectedNode?.Node, out bool editorChanged, out bool textChanged);

        BuildAddDropDownMenu();
        _buttonRemove.Enabled = _treeView.Nodes is [var node, ..] && e.Node != node && _presenter.GetRemoveCommand() != null;

        if (editorChanged) UpdateEditorControl();
        else _editorControl?.Refresh();

        if (textChanged) _textEditor.SetContent(GetSerialized(), Serializer);

        SelectionChanged?.Invoke();
    }

    private Control? _editorControl;

    private void UpdateEditorControl()
    {
        if (_editorControl != null)
        {
            _editorPanel.Controls.Remove(_editorControl);
            _editorControl.Dispose();
            _editorControl = null;
        }

        if (_presenter.GetEditorControl() is {} editor)
        {
            _editorControl = (Control)editor;
            _editorControl.Dock = DockStyle.Fill;
            _editorPanel.Controls.Add(_editorControl);
        }
    }

    /// <summary>
    /// Returns the serialized representation of the selected <see cref="StructureTreeNode"/>.
    /// </summary>
    protected virtual string GetSerialized()
        => _presenter.GetSerialized();

    private void TextEditorContentChanged(string text)
    {
        if (_presenter.UpdateFromText(text))
            _textEditor.ClearUndoStack();
    }
    #endregion
}