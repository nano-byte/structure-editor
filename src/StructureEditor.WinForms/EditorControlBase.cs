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
    /// Common base class for <see cref="IEditorControl{T}"/> implementations.
    /// </summary>
    /// <typeparam name="T">The type of element to edit.</typeparam>
    public abstract class EditorControlBase<T> : UserControl, IEditorControl<T> where T : class
    {
        #region Properties
        private T _target;

        /// <inheritdoc/>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual T Target
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
        protected event Action TargetChanged;

        private ICommandExecutor _commandExecutor;

        /// <inheritdoc/>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual ICommandExecutor CommandExecutor
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
        protected event Action CommandExecutorChanged;
        #endregion

        #region Constructor
        protected EditorControlBase()
        {
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            AutoScroll = true;
        }
        #endregion

        //--------------------//

        #region Refresh
        /// <summary>
        /// Is raised when <see cref="Refresh"/> is called.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Is not really an event but rather a hook.")]
        protected event Action OnRefresh;

        public override void Refresh()
        {
            OnRefresh?.Invoke();
            base.Refresh();
        }
        #endregion

        #region Register
        /// <summary>
        /// Hooks a WinForms control in to the live editing and Undo system.
        /// </summary>
        /// <param name="control">The control to hook up (is automatically added to <see cref="Control.Controls"/>).</param>
        /// <param name="pointer">Read/write access to the value the <paramref name="control"/> represents.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The set-value callback method may throw any kind of exception.")]
        protected void RegisterControl(Control control, PropertyPointer<string> pointer)
        {
            Controls.Add(control);

            control.Validated += delegate
            {
                string text = string.IsNullOrEmpty(control.Text) ? null : control.Text;
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
        /// Hooks a <see cref="ComboBox"/> in to the live editing and Undo system.
        /// </summary>
        /// <param name="control">The control to hook up (is automatically added to <see cref="Control.Controls"/>).</param>
        /// <param name="pointer">Read/write access to the value the <paramref name="control"/> represents.</param>
        protected void RegisterControl(ComboBox control, PropertyPointer<string> pointer)
        {
            // Setting ComboBox.Text will only work reliably if the value is in the Items list
            OnRefresh += () =>
            {
                if (pointer.Value != null && !control.Items.Contains(pointer.Value))
                    control.Items.Add(pointer.Value);
            };

            RegisterControl((Control)control, pointer);
        }

        /// <summary>
        /// Hooks a <see cref="UriTextBox"/> in to the live editing and Undo system.
        /// </summary>
        /// <param name="control">The control to hook up (is automatically added to <see cref="Control.Controls"/>).</param>
        /// <param name="pointer">Read/write access to the value the <paramref name="control"/> represents.</param>
        protected void RegisterControl(UriTextBox control, PropertyPointer<Uri> pointer)
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
        /// Hooks up a <see cref="IEditorControl{T}"/> as child editor.
        /// </summary>
        /// <typeparam name="TControl">The specific <see cref="IEditorControl{T}"/> type.</typeparam>
        /// <typeparam name="TChild">The type the child editor handles.</typeparam>
        /// <param name="control">The control to hook up (is automatically added to <see cref="Control.Controls"/>).</param>
        /// <param name="getTarget">Callback to retrieve the (child) target of the <paramref name="control"/>.</param>
        protected void RegisterControl<TControl, TChild>(TControl control, Func<TChild> getTarget)
            where TControl : Control, IEditorControl<TChild>
        {
            Controls.Add(control);

            TargetChanged += () => control.Target = getTarget();
            CommandExecutorChanged += () => control.CommandExecutor = CommandExecutor;
            OnRefresh += control.Refresh;
        }

        /// <summary>
        /// Hooks a <see cref="CheckBox"/> in to the live editing and Undo system.
        /// </summary>
        /// <param name="control">The control to hook up (is automatically added to <see cref="Control.Controls"/>).</param>
        /// <param name="pointer">Read/write access to the value the <paramref name="control"/> represents.</param>
        protected void RegisterControl(CheckBox control, PropertyPointer<bool> pointer)
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
        #endregion
    }
}
