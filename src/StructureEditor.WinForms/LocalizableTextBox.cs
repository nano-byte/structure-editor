// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using NanoByte.Common.Collections;
using NanoByte.Common.Undo;
using NanoByte.Common.Values;

namespace NanoByte.StructureEditor.WinForms
{
    /// <summary>
    /// A control for editing a <see cref="LocalizableStringCollection"/>.
    /// </summary>
    public class LocalizableTextBox : EditorControlBase<LocalizableStringCollection>
    {
        private readonly System.Windows.Forms.ComboBox _comboBoxLanguage = new System.Windows.Forms.ComboBox
        {
            DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList,
            DropDownWidth = 86,
            FormattingEnabled = true,
            Location = new Point(0, 0),
            Size = new Size(60, 21)
        };

        public Common.Controls.HintTextBox TextBox { get; } = new Common.Controls.HintTextBox
        {
            Anchor = System.Windows.Forms.AnchorStyles.Top
                   | System.Windows.Forms.AnchorStyles.Bottom
                   | System.Windows.Forms.AnchorStyles.Left
                   | System.Windows.Forms.AnchorStyles.Right,
            Location = new Point(66, 0),
            Multiline = true,
            ShowClearButton = true,
            Size = new Size(84, 150)
        };

        private CultureInfo _selectedLanguage;
        private bool _textBoxDirty;

        /// <summary>
        /// Controls whether the text can span more than one line.
        /// </summary>
        [DefaultValue(true)]
        [Category("Behavior"), Description("Controls whether the text can span more than one line.")]
        public bool Multiline { get => TextBox.Multiline; set => TextBox.Multiline = value; }

        /// <summary>
        /// A text to be displayed in <see cref="SystemColors.GrayText"/> when the text box is empty.
        /// </summary>
        [Description("A text to be displayed in gray when Text is empty."), Category("Appearance"), Localizable(true)]
        public string HintText { get => TextBox.HintText; set => TextBox.HintText = value; }

        public LocalizableTextBox()
        {
            _comboBoxLanguage.SelectionChangeCommitted += (sender, e) =>
            {
                _selectedLanguage = (CultureInfo)_comboBoxLanguage.SelectedItem;
                FillTextBox();
            };

            TextBox.TextChanged += (sender, e) => _textBoxDirty = true;

            TextBox.Validating += (sender, e) =>
            {
                if (_selectedLanguage == null) return;

                if (_textBoxDirty)
                {
                    ApplyValue();
                    _textBoxDirty = false;
                }
            };

            SuspendLayout();
            Controls.Add(_comboBoxLanguage);
            Controls.Add(TextBox);
            MinimumSize = new Size(65, 22);
            ResumeLayout(false);
            PerformLayout();
        }

        private void FillComboBox()
        {
            var setLanguages = new List<CultureInfo>();
            var unsetLanguages = new List<CultureInfo>();
            foreach (var language in Languages.AllKnown)
            {
                if (Target.ContainsExactLanguage(language)) setLanguages.Add(language);
                else unsetLanguages.Add(language);
            }

            if (_selectedLanguage == null)
                _selectedLanguage = setLanguages.Count == 0 ? LocalizableString.DefaultLanguage : setLanguages[0];

            _comboBoxLanguage.BeginUpdate();
            _comboBoxLanguage.Items.Clear();
            foreach (var language in setLanguages) _comboBoxLanguage.Items.Add(language);
            foreach (var language in unsetLanguages) _comboBoxLanguage.Items.Add(language);
            _comboBoxLanguage.SelectedItem = _selectedLanguage;
            _comboBoxLanguage.EndUpdate();
        }

        private void FillTextBox()
        {
            try
            {
                TextBox.Text = Target.GetExactLanguage(_selectedLanguage);
            }
            catch (KeyNotFoundException)
            {
                TextBox.Text = "";
            }
            _textBoxDirty = false;
        }

        private void ApplyValue()
        {
            string newValue = string.IsNullOrEmpty(TextBox.Text) ? null : TextBox.Text;

            if (Target.GetExactLanguage(_selectedLanguage) == newValue) return;

            if (CommandExecutor == null) Target.Set(_selectedLanguage, newValue);
            else CommandExecutor.Execute(new SetLocalizableString(Target, new LocalizableString {Language = _selectedLanguage, Value = newValue}));
        }

        public override void Refresh()
        {
            FillComboBox();
            FillTextBox();

            base.Refresh();
        }
    }
}
