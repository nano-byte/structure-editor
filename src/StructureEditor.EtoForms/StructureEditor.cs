// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using NanoByte.Common.Undo;

namespace NanoByte.StructureEditor.EtoForms;

/// <summary>
/// An editor for hierarchical structures.
/// </summary>
/// <remarks>Derive and call <see cref="DescribeRoot"/> or <see cref="DescribeRoot{TEditor}"/> as well as <see cref="Describe{TContainer}"/> in the constructor.</remarks>
/// <typeparam name="T">The type of object to edit.</typeparam>
public class StructureEditor<T> : Panel, IStructureEditor<T>, IPendingChanges
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
        _presenter.StructureChanged += RebuildLater;

        _textEditor = new(TextEditorFactory.Default());
        SetupControls();
    }

    /// <summary>
    /// Creates a new structure editor for a <typeparamref name="T"/> with a parameterless constructor.
    /// </summary>
    public StructureEditor() : this(Activator.CreateInstance<T>) {}

    #region Controls
    private readonly Button _buttonAdd = new() {Text = "Add", Enabled = false};
    private readonly Button _buttonRemove = new() {Text = "Remove", Enabled = false};

    // Small preferred size, since the grid otherwise asks for the width of its longest entry and the height of all rows, growing its window
    private readonly TreeGridView _treeView = new() {ShowHeader = false, Size = new(100, 100)};

    private readonly Panel _editorPanel = new();
    private readonly ValidatingTextEditor _textEditor;

    private void SetupControls()
    {
        _buttonAdd.Click += delegate { ShowAddMenu(); };
        _buttonRemove.Click += delegate { Remove(); };

        _treeView.Columns.Add(new GridColumn
        {
            Expand = true,
            DataCell = new TextBoxCell {Binding = Binding.Delegate<TreeItem, string>(x => x.Text)},
            CellToolTipBinding = Binding.Delegate<TreeItem, string>(x => x.ToolTip)
        });
        _treeView.SelectedItemChanged += delegate
        {
            // Ignore transient deselection (e.g., while the tree is being reloaded) and repeated notifications
            if (!_suppressSelectionEvents && _treeView.SelectedItem is TreeItem item && item != _selectedItem)
                OnSelected(item, commitPending: true);
        };

        _textEditor.ContentChanged += text => _presenter.UpdateFromText(text);

        Content = new Splitter
        {
            Orientation = Orientation.Vertical,
            FixedPanel = SplitterFixedPanel.None,
            RelativePosition = 0.5,
            Panel1 = new Splitter
            {
                Orientation = Orientation.Horizontal,
                FixedPanel = SplitterFixedPanel.None,
                RelativePosition = 0.4,
                Panel1 = new TableLayout
                {
                    Spacing = new(0, 4),
                    Rows =
                    {
                        new TableRow(new StackLayout
                        {
                            Orientation = Orientation.Horizontal,
                            Spacing = 4,
                            Items = {_buttonAdd, _buttonRemove}
                        }),
                        new TableRow(_treeView) {ScaleHeight = true}
                    }
                },
                Panel2 = _editorPanel
            },
            Panel2 = _textEditor
        };
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
        => DescribeRoot<PropertyEditorNodeEditor<T>>(name);
    #endregion

    #region Target
    /// <summary>
    /// Holds the object being edited and manages undo/redo operations on it.
    /// </summary>
    public ICommandManager<T> CommandManager => _presenter.CommandManager;

    /// <summary>
    /// The serializer used for the text representation of the selected node. Defaults to <see cref="XmlStructureSerializer"/>.
    /// </summary>
    /// <remarks>Set automatically by <see cref="Open"/> when passed a <see cref="SerializingCommandManager{T}"/>.</remarks>
    public IStructureSerializer Serializer
    {
        get => _presenter.Serializer;
        set
        {
            CommitPendingChanges();
            _presenter.Serializer = value;
            _textEditor.SetContent(_presenter.GetSerialized(), value);
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
        _rebuildScheduled = false;

        _presenter.Open(commandManager);

        RebuildTree();
        _textEditor.SetContent(_presenter.GetSerialized(), Serializer);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _rebuildScheduled = false;
            _presenter.StructureChanged -= RebuildLater;
            _presenter.Close();
        }
        base.Dispose(disposing);
    }
    #endregion

    #region Build tree
    /// <summary>
    /// A single item in the <see cref="_treeView"/>.
    /// </summary>
    private sealed class TreeItem(Node node, TreeItem? parent) : TreeGridItem
    {
        public Node Node => node;
        public TreeItem? ParentItem => parent;
        public string Text { get; } = node.ToString();
        public string ToolTip => node.Description ?? "";
    }

    private TreeGridItemCollection _treeItems = [];

    private bool _rebuildScheduled;

    /// <summary>
    /// Coalesces multiple change notifications into a single <see cref="RebuildTree"/>.
    /// </summary>
    private void RebuildLater()
    {
        if (_rebuildScheduled) return;
        _rebuildScheduled = true;

        Application.Instance.AsyncInvoke(() =>
        {
            if (!_rebuildScheduled) return;
            _rebuildScheduled = false;
            RebuildTree();
        });
    }


    /// <summary>
    /// Rebuilds the <see cref="_treeView"/> while attempting to retain the current selection and expansion state.
    /// </summary>
    private void RebuildTree()
    {
        var expandedTargets = GetExpandedTargets();
        TreeItem? reselectItem = null;

        void Add(TreeGridItemCollection collection, IEnumerable<Node> nodes, TreeItem? parent)
        {
            foreach (var node in nodes)
            {
                var item = new TreeItem(node, parent) {Expanded = node.Target != null && expandedTargets.Contains(node.Target)};
                if (_presenter.ShouldSelect(node)) reselectItem = item;
                Add(item.Children, _presenter.GetNodes(node.Target), item);
                collection.Add(item);
            }
        }

        var items = new TreeGridItemCollection();
        Add(items, _presenter.GetRootNodes(), parent: null);
        reselectItem ??= items.OfType<TreeItem>().FirstOrDefault();

        // Ensure the selected item is visible and shows its children
        for (var item = reselectItem; item != null; item = item.ParentItem)
            item.Expanded = true;

        _treeItems = items;
        _suppressSelectionEvents = true;
        try
        {
            ReplaceTreeContent(items);
        }
        finally
        {
            _suppressSelectionEvents = false;
        }
        // Do not apply pending input here: The controls may still show values from before the change that caused the rebuild (e.g., an undo)
        SelectItem(reselectItem);
    }
    private void ReplaceTreeContent(TreeGridItemCollection items)
    {
        // The WinForms backend may repaint rows while they are being removed, which fails because their items are already gone
        bool hide = Eto.Platform.Instance.IsWinForms && _treeView.Loaded;
        bool hadFocus = hide && _treeView.HasFocus;
        if (hide) _treeView.Visible = false;
        try
        {
            _treeView.DataStore = items;
        }
        finally
        {
            if (hide)
            {
                _treeView.Visible = true;
                if (hadFocus) _treeView.Focus();
            }
        }
    }

    /// <summary>
    /// Collects the <see cref="Node.Target"/>s of all currently expanded tree items.
    /// </summary>
    private HashSet<object> GetExpandedTargets()
    {
        var targets = new HashSet<object>(StructureEditorPresenter<T>.TargetComparer);

        void Collect(TreeGridItemCollection collection)
        {
            foreach (var item in collection.OfType<TreeItem>())
            {
                if (item is {Expanded: true, Node.Target: {} target}) targets.Add(target);
                Collect(item.Children);
            }
        }

        Collect(_treeItems);
        return targets;
    }
    #endregion

    #region Undo
    /// <summary>
    /// Undoes the last action.
    /// </summary>
    public void Undo()
    {
        CommitPendingChanges();
        if (CommandManager.UndoEnabled) CommandManager.Undo();
    }

    /// <summary>
    /// Redoes the last action.
    /// </summary>
    public void Redo()
    {
        CommitPendingChanges();
        if (CommandManager.RedoEnabled) CommandManager.Redo();
    }

    /// <summary>
    /// Applies any user input in the node editor or text editor that has not been applied yet (e.g., because the control still has focus).
    /// </summary>
    /// <remarks>Call this before reading or saving the <see cref="CommandManager"/>'s target.</remarks>
    public void CommitPendingChanges()
    {
        (_editorPanel.Content as IPendingChanges)?.CommitPendingChanges();
        _textEditor.CommitPendingChanges();
    }
    #endregion

    //--------------------//

    #region Add/remove
    /// <summary>
    /// Applies pending input and ensures the tree and selection reflect the result, so that the selected node is not stale.
    /// </summary>
    private void CommitAndRebuild()
    {
        CommitPendingChanges();

        if (_rebuildScheduled)
        {
            _rebuildScheduled = false;
            RebuildTree();
        }
    }

    private void ShowAddMenu()
    {
        CommitAndRebuild();

        var menu = new ContextMenu();
        foreach (var candidate in _presenter.GetCandidates())
        {
            if (candidate == null) menu.Items.Add(new SeparatorMenuItem());
            else
            {
                var item = new ButtonMenuItem {Text = candidate.NodeType, ToolTip = candidate.Description ?? ""};
                item.Click += delegate { _presenter.Add(candidate); };
                menu.Items.Add(item);
            }
        }
        if (menu.Items.Count != 0) menu.Show(_buttonAdd, new PointF(0, _buttonAdd.Height));
    }

    /// <summary>
    /// Removes the currently selected entry.
    /// </summary>
    public void Remove()
    {
        CommitAndRebuild();

        // The root has no parent and cannot be removed
        if (_selectedItem is not {ParentItem: {} parent}) return;

        var removeCommand = _presenter.GetRemoveCommand();
        if (removeCommand == null) return;

        SelectItem(parent); // Select parent before deleting
        CommandManager.Execute(removeCommand);
    }
    #endregion

    #region Selection
    /// <summary>
    /// The target object of the currently selected node; <c>null</c> if nothing is selected.
    /// </summary>
    public object? SelectedTarget => _presenter.SelectedTarget;

    /// <summary>
    /// Raised when the selected node in the tree changes.
    /// </summary>
    public event EventHandler<EventArgs>? SelectionChanged;

    /// <summary>The tree item the editor controls currently represent.</summary>
    private TreeItem? _selectedItem;

    /// <summary>Suppresses reacting to selection changes in the <see cref="_treeView"/> while it is modified programmatically.</summary>
    private bool _suppressSelectionEvents;

    /// <summary>
    /// Selects an item in the <see cref="_treeView"/> and updates the editor controls accordingly.
    /// </summary>
    private void SelectItem(TreeItem? item)
    {
        void SetTreeSelection()
        {
            _suppressSelectionEvents = true;
            try
            {
                _treeView.SelectedItem = item;

                // The WinForms backend discards the first selection made after the tree content was replaced, so select another row first
                if (item != null && !ReferenceEquals(_treeView.SelectedItem, item) && GetVisibleRow(item) is var row and >= 0)
                {
                    if (row != 0) _treeView.SelectRow(0);
                    else if (item is {Expanded: true, Children.Count: > 0}) _treeView.SelectRow(1);
                    _treeView.SelectRow(row);
                }
            }
            finally
            {
                _suppressSelectionEvents = false;
            }
        }

        SetTreeSelection();
        OnSelected(item, commitPending: false);

        // Some platforms only apply the selection once they have finished loading new tree content
        if (item != null && !ReferenceEquals(_treeView.SelectedItem, item))
        {
            Application.Instance.AsyncInvoke(() =>
            {
                if (_selectedItem == item) SetTreeSelection();
            });
        }
    }

    /// <summary>
    /// Determines the index of the row an item is currently displayed in; -1 if it is hidden within a collapsed item.
    /// </summary>
    private int GetVisibleRow(TreeItem item)
    {
        int row = 0;

        bool Find(TreeGridItemCollection collection)
        {
            foreach (var candidate in collection.OfType<TreeItem>())
            {
                if (candidate == item) return true;
                row++;
                if (candidate.Expanded && Find(candidate.Children)) return true;
            }
            return false;
        }

        return Find(_treeItems) ? row : -1;
    }

    private void OnSelected(TreeItem? item, bool commitPending)
    {
        // Apply input for the previously selected node before its editor is replaced
        if (commitPending) CommitPendingChanges();

        _selectedItem = item;
        _presenter.Select(item?.Node, out bool editorChanged, out bool textChanged);

        _buttonAdd.Enabled = _presenter.GetCandidates().Count != 0;
        _buttonRemove.Enabled = item?.ParentItem != null && _presenter.GetRemoveCommand() != null;

        if (editorChanged) UpdateEditorControl();
        else (_editorPanel.Content as INodeEditor)?.Refresh();

        if (textChanged) _textEditor.SetContent(_presenter.GetSerialized(), Serializer);

        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }
    private void UpdateEditorControl()
    {
        var oldControl = _editorPanel.Content;
        _editorPanel.Content = (Control?)_presenter.GetEditorControl();

        // Defer, since the platform may still have callbacks queued for the control (e.g., for it being shown)
        if (oldControl != null) Application.Instance.AsyncInvoke(oldControl.Dispose);
    }
    #endregion
}
