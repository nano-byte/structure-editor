// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Diagnostics.CodeAnalysis;
using Eto.Forms;
using NanoByte.Common;
using NanoByte.Common.EtoControls;
using NanoByte.Common.Undo;
using ICommandExecutor = NanoByte.Common.Undo.ICommandExecutor;

namespace NanoByte.StructureEditor.EtoForms;

/// <summary>
/// Common base class for controls that edit a node in the structure.
/// </summary>
/// <remarks>
/// Use the <c>Bind()</c> methods to connect controls to properties of the <see cref="Target"/> and then place them in a layout assigned to <see cref="Panel.Content"/>.
/// </remarks>
/// <typeparam name="T">The type of element to edit.</typeparam>
public abstract class NodeEditorBase<T> : Panel, INodeEditor<T>, IPendingChanges
    where T : class
{
    private T? _target;

    /// <inheritdoc/>
    public virtual T? Target
    {
        get => _target;
        set
        {
            _target = value;
            TargetChanged?.Invoke();
            Refresh();
        }
    }

    /// <summary>
    /// Is raised when <see cref="Target"/> has been changed.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Is not really an event but rather a hook.")]
    protected event Action? TargetChanged;

    private ICommandExecutor? _commandExecutor;

    /// <inheritdoc/>
    public virtual ICommandExecutor? CommandExecutor
    {
        get => _commandExecutor;
        set
        {
            _commandExecutor = value;
            CommandExecutorChanged?.Invoke();
        }
    }

    /// <summary>
    /// Is raised when <see cref="CommandExecutor"/> has been changed.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Is not really an event but rather a hook.")]
    protected event Action? CommandExecutorChanged;

    /// <summary>
    /// Is raised when <see cref="Refresh"/> is called.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Is not really an event but rather a hook.")]
    protected event Action? OnRefresh;

    /// <summary>Indicates that controls are currently being updated programmatically, so their change events must not be treated as user input.</summary>
    private bool _refreshing;

    /// <inheritdoc/>
    public virtual void Refresh()
    {
        _refreshing = true;
        try
        {
            OnRefresh?.Invoke();
        }
        finally
        {
            _refreshing = false;
        }
    }

    /// <summary>
    /// Is raised when <see cref="CommitPendingChanges"/> is called.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Is not really an event but rather a hook.")]
    protected event Action? OnCommitPendingChanges;

    /// <inheritdoc/>
    public virtual void CommitPendingChanges()
    {
        if (Target != null && !_refreshing) OnCommitPendingChanges?.Invoke();
    }

    /// <summary>
    /// Sets a new value through the Undo system, if available.
    /// </summary>
    /// <param name="pointer">Read/write access to the value.</param>
    /// <param name="value">The new value.</param>
    protected void SetValue<TValue>(PropertyPointer<TValue> pointer, TValue value)
    {
        if (CommandExecutor == null) pointer.Value = value;
        else CommandExecutor.Execute(SetValueCommand.For(pointer, value));
    }

    /// <summary>
    /// Binds a text control (e.g., a <see cref="TextBox"/> or <see cref="TextArea"/>) to a <see cref="string"/> property through the live editing and Undo system.
    /// </summary>
    /// <remarks>Input is applied when the control loses focus, when Enter is pressed in a single-line control and when <see cref="CommitPendingChanges"/> is called.</remarks>
    /// <param name="control">The control to hook up.</param>
    /// <param name="pointer">Read/write access to the value the <paramref name="control"/> represents.</param>
    /// <returns>The <paramref name="control"/>, for placing in a layout.</returns>
    protected TControl Bind<TControl>(TControl control, PropertyPointer<string?> pointer)
        where TControl : TextControl
    {
        BindText(control, pointer, () => control.Text, text => control.Text = text, commitOnEnter: control is not TextArea);
        return control;
    }

    /// <summary>
    /// Binds an editable <see cref="ComboBox"/> to a <see cref="string"/> property through the live editing and Undo system.
    /// </summary>
    /// <param name="control">The control to hook up.</param>
    /// <param name="pointer">Read/write access to the value the <paramref name="control"/> represents.</param>
    /// <returns>The <paramref name="control"/>, for placing in a layout.</returns>
    protected ComboBox Bind(ComboBox control, PropertyPointer<string?> pointer)
    {
        BindText(control, pointer, () => control.Text, text => control.Text = text, commitOnEnter: true);
        control.SelectedValueChanged += delegate { CommitPendingChanges(); };
        return control;
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The set-value callback method may throw any kind of exception.")]
    private void BindText(Control control, PropertyPointer<string?> pointer, Func<string?> getText, Action<string> setText, bool commitOnEnter)
    {
        void Commit()
        {
            string? text = getText();
            if (string.IsNullOrEmpty(text)) text = null;
            if (text == pointer.Value) return;

            try
            {
                SetValue(pointer, text);
            }
            #region Error handling
            catch (Exception ex)
            {
                setText(pointer.Value ?? "");
                Msg.Inform(this, ex.Message, MsgSeverity.Warn);
            }
            #endregion
        }

        OnCommitPendingChanges += Commit;
        control.LostFocus += delegate { CommitPendingChanges(); };
        if (commitOnEnter)
        {
            control.KeyDown += (_, e) =>
            {
                if (e.Key == Keys.Enter) CommitPendingChanges();
            };
        }

        OnRefresh += () =>
        {
            string text = pointer.Value ?? "";
            if (getText() != text) setText(text);
        };
    }

    /// <summary>
    /// Binds a <see cref="UriTextBox"/> to an <see cref="Uri"/> property through the live editing and Undo system.
    /// </summary>
    /// <param name="control">The control to hook up.</param>
    /// <param name="pointer">Read/write access to the value the <paramref name="control"/> represents.</param>
    /// <returns>The <paramref name="control"/>, for placing in a layout.</returns>
    protected UriTextBox Bind(UriTextBox control, PropertyPointer<Uri?> pointer)
    {
        void Commit()
        {
            if (!control.IsValid || control.Uri == pointer.Value) return;
            SetValue(pointer, control.Uri);
        }

        OnCommitPendingChanges += Commit;
        control.LostFocus += delegate { CommitPendingChanges(); };
        control.KeyDown += (_, e) =>
        {
            if (e.Key == Keys.Enter) CommitPendingChanges();
        };

        OnRefresh += () => control.Uri = pointer.Value;
        return control;
    }

    /// <summary>
    /// Binds a <see cref="CheckBox"/> to a <see cref="bool"/> property through the live editing and Undo system.
    /// </summary>
    /// <param name="control">The control to hook up.</param>
    /// <param name="pointer">Read/write access to the value the <paramref name="control"/> represents.</param>
    /// <returns>The <paramref name="control"/>, for placing in a layout.</returns>
    protected CheckBox Bind(CheckBox control, PropertyPointer<bool> pointer)
    {
        control.CheckedChanged += delegate
        {
            bool value = control.Checked == true;
            if (Target == null || _refreshing || value == pointer.Value) return;
            SetValue(pointer, value);
        };

        OnRefresh += () => control.Checked = pointer.Value;
        return control;
    }

    /// <summary>
    /// Binds a <see cref="INodeEditor{T}"/> as child editor through the live editing and Undo system.
    /// </summary>
    /// <typeparam name="TControl">The specific <see cref="INodeEditor{T}"/> type.</typeparam>
    /// <typeparam name="TChild">The type the child editor handles.</typeparam>
    /// <param name="control">The control to hook up.</param>
    /// <param name="getTarget">Callback to retrieve the (child) target of the <paramref name="control"/>.</param>
    /// <returns>The <paramref name="control"/>, for placing in a layout.</returns>
    protected TControl Bind<TControl, TChild>(TControl control, Func<TChild> getTarget)
        where TControl : Control, INodeEditor<TChild>
        where TChild : class
    {
        TargetChanged += () => control.Target = getTarget();
        CommandExecutorChanged += () => control.CommandExecutor = CommandExecutor;
        OnRefresh += control.Refresh;
        if (control is IPendingChanges pendingChanges) OnCommitPendingChanges += pendingChanges.CommitPendingChanges;
        return control;
    }
}
