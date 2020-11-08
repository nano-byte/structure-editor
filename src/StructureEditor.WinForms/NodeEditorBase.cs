// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using NanoByte.Common;
using NanoByte.Common.Controls;
using NanoByte.Common.Undo;
using ICommandExecutor = NanoByte.Common.Undo.ICommandExecutor;

namespace NanoByte.StructureEditor.WinForms
{
    /// <summary>
    /// Common base class for controls that edits a node in the structure.
    /// </summary>
    /// <typeparam name="T">The type of element to edit.</typeparam>
    public abstract class NodeEditorBase<T> : UserControl, INodeEditor<T>
        where T : class
    {
        private T? _target;

        /// <inheritdoc/>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

        protected NodeEditorBase()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            AutoScroll = true;
        }

        /// <summary>
        /// Is raised when <see cref="Refresh"/> is called.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Is not really an event but rather a hook.")]
        protected event Action? OnRefresh;

        public override void Refresh()
        {
            OnRefresh?.Invoke();
            base.Refresh();
        }

        /// <summary>
        /// Binds a WinForms control to a property through the live editing and Undo system.
        /// </summary>
        /// <param name="control">The control to hook up (is automatically added to <see cref="Control.Controls"/>).</param>
        /// <param name="pointer">Read/write access to the value the <paramref name="control"/> represents.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The set-value callback method may throw any kind of exception.")]
        protected void Bind(Control control, PropertyPointer<string?> pointer)
        {
            Controls.Add(control);

            control.Validated += delegate
            {
                string? text = string.IsNullOrEmpty(control.Text) ? null : control.Text;
                if (text == pointer.Value) return;

                try
                {
                    if (CommandExecutor == null) pointer.Value = text;
                    else CommandExecutor.Execute(SetValueCommand.For(pointer, text));
                }
                #region Error handling
                catch (Exception ex)
                {
                    Msg.Inform(this, ex.Message, MsgSeverity.Warn);
                    control.Text = pointer.Value;
                }
                #endregion
            };

            OnRefresh += () =>
            {
                if (control.Text != pointer.Value) control.Text = pointer.Value;
            };
        }

        /// <summary>
        /// Binds a <see cref="ComboBox"/> to a <see cref="string"/> property through the live editing and Undo system.
        /// </summary>
        /// <param name="control">The control to hook up (is automatically added to <see cref="Control.Controls"/>).</param>
        /// <param name="pointer">Read/write access to the value the <paramref name="control"/> represents.</param>
        protected void Bind(ComboBox control, PropertyPointer<string?> pointer)
        {
            // Setting ComboBox.Text will only work reliably if the value is in the Items list
            OnRefresh += () =>
            {
                if (pointer.Value != null && !control.Items.Contains(pointer.Value))
                    control.Items.Add(pointer.Value);
            };

            Bind((Control)control, pointer);
        }

        /// <summary>
        /// Binds a <see cref="UriTextBox"/> to an <see cref="Uri"/> property through the live editing and Undo system.
        /// </summary>
        /// <param name="control">The control to hook up (is automatically added to <see cref="Control.Controls"/>).</param>
        /// <param name="pointer">Read/write access to the value the <paramref name="control"/> represents.</param>
        protected void Bind(UriTextBox control, PropertyPointer<Uri?> pointer)
        {
            Controls.Add(control);

            control.Validated += delegate
            {
                if (!control.IsValid || control.Uri == pointer.Value) return;

                if (CommandExecutor == null) pointer.Value = control.Uri;
                else CommandExecutor.Execute(SetValueCommand.For(pointer, control.Uri));
            };

            OnRefresh += () => control.Uri = pointer.Value;
        }

        /// <summary>
        /// Binds a <see cref="INodeEditor{T}"/> as child editor through the live editing and Undo system.
        /// </summary>
        /// <typeparam name="TControl">The specific <see cref="INodeEditor{T}"/> type.</typeparam>
        /// <typeparam name="TChild">The type the child editor handles.</typeparam>
        /// <param name="control">The control to hook up (is automatically added to <see cref="Control.Controls"/>).</param>
        /// <param name="getTarget">Callback to retrieve the (child) target of the <paramref name="control"/>.</param>
        protected void Bind<TControl, TChild>(TControl control, Func<TChild> getTarget)
            where TControl : Control, INodeEditor<TChild>
            where TChild : class
        {
            Controls.Add(control);

            TargetChanged += () => control.Target = getTarget();
            CommandExecutorChanged += () => control.CommandExecutor = CommandExecutor;
            OnRefresh += control.Refresh;
        }

        /// <summary>
        /// Binds a <see cref="CheckBox"/> to a <see cref="bool"/> property through the live editing and Undo system.
        /// </summary>
        /// <param name="control">The control to hook up (is automatically added to <see cref="Control.Controls"/>).</param>
        /// <param name="pointer">Read/write access to the value the <paramref name="control"/> represents.</param>
        protected void Bind(CheckBox control, PropertyPointer<bool> pointer)
        {
            Controls.Add(control);

            control.CheckedChanged += delegate
            {
                if (control.Checked == pointer.Value) return;

                if (CommandExecutor == null) pointer.Value = control.Checked;
                else CommandExecutor.Execute(SetValueCommand.For(pointer, control.Checked));
            };

            OnRefresh += () => control.Checked = pointer.Value;
        }
    }
}
