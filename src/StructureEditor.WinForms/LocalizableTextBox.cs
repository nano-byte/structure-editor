// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using NanoByte.Common.Collections;
using NanoByte.Common.Controls;
using NanoByte.Common.Undo;
using NanoByte.Common.Values;

namespace NanoByte.StructureEditor.WinForms
{
    /// <summary>
    /// Exposes a <see cref="LocalizableStringCollection"/> as a text box with a drop-down box for language switching.
    /// </summary>
    public class LocalizableTextBox : NodeEditorBase<LocalizableStringCollection>
    {
        private readonly ComboBox _comboBoxLanguage = new()
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            DropDownWidth = 86,
            FormattingEnabled = true,
            Location = new Point(0, 0),
            Size = new Size(60, 21)
        };

        public HintTextBox TextBox { get; } = new()
        {
            Anchor = AnchorStyles.Top
                   | AnchorStyles.Bottom
                   | AnchorStyles.Left
                   | AnchorStyles.Right,
            Location = new Point(66, 0),
            Multiline = true,
            ShowClearButton = true,
            Size = new Size(84, 150)
        };

        private CultureInfo? _selectedLanguage;
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
            SuspendLayout();
            SetupControls();
            ResumeLayout(performLayout: false);
        }

        private void SetupControls()
        {
            MinimumSize = new Size(65, 22);

            _comboBoxLanguage.SelectionChangeCommitted += (_, _) =>
            {
                _selectedLanguage = (CultureInfo) _comboBoxLanguage.SelectedItem;
                FillTextBox();
            };
            Controls.Add(_comboBoxLanguage);

            TextBox.TextChanged += (_, _) => _textBoxDirty = true;
            TextBox.Validating += (_, _) =>
            {
                if (_selectedLanguage == null) return;

                if (_textBoxDirty)
                {
                    ApplyValue();
                    _textBoxDirty = false;
                }
            };
            Controls.Add(TextBox);
        }

        private void FillComboBox()
        {
            var setLanguages = new List<CultureInfo>();
            var unsetLanguages = new List<CultureInfo>();
            foreach (var language in Languages.AllKnown)
            {
                if (Target != null && Target.ContainsExactLanguage(language)) setLanguages.Add(language);
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
                TextBox.Text = (_selectedLanguage == null) ? "" : Target?.GetExactLanguage(_selectedLanguage) ?? "";
            }
            catch (KeyNotFoundException)
            {
                TextBox.Text = "";
            }
            _textBoxDirty = false;
        }

        private void ApplyValue()
        {
            string? newValue = string.IsNullOrEmpty(TextBox.Text) ? null : TextBox.Text;

            if (_selectedLanguage == null || Target == null || Target.GetExactLanguage(_selectedLanguage) == newValue) return;

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
