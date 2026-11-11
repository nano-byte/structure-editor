// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Eto.Forms;
using NanoByte.Common.Collections;
using NanoByte.Common.Undo;
using NanoByte.Common.Values;

namespace NanoByte.StructureEditor.EtoForms;

/// <summary>
/// Exposes a <see cref="LocalizableStringCollection"/> as a text box with a drop-down box for language switching.
/// </summary>
public class LocalizableTextBox : NodeEditorBase<LocalizableStringCollection>
{
    private readonly DropDown _dropDownLanguage = new()
    {
        ItemTextBinding = Binding.Delegate<CultureInfo, string>(x => x.ToString())
    };

    /// <summary>
    /// The control used to enter the text. A <see cref="TextArea"/> if multiple lines are allowed, a <see cref="TextBox"/> otherwise.
    /// </summary>
    public TextControl TextControl { get; }

    private CultureInfo? _selectedLanguage;

    /// <summary>Suppresses <see cref="ListControl.SelectedValueChanged"/> while <see cref="_dropDownLanguage"/> is filled programmatically.</summary>
    private bool _fillingDropDown;

    /// <summary>
    /// A text to be displayed when the text box is empty. Only supported for single-line text boxes.
    /// </summary>
    public string? PlaceholderText
    {
        get => (TextControl as TextBox)?.PlaceholderText;
        set
        {
            if (TextControl is TextBox textBox) textBox.PlaceholderText = value;
        }
    }

    /// <summary>
    /// Creates a new localizable text box that allows multiple lines.
    /// </summary>
    public LocalizableTextBox() : this(multiline: true) {}

    /// <summary>
    /// Creates a new localizable text box.
    /// </summary>
    /// <param name="multiline">Controls whether the text can span more than one line.</param>
    public LocalizableTextBox(bool multiline)
    {
        TextControl = multiline ? new TextArea {Size = new(100, 60)} : new TextBox();

        _dropDownLanguage.SelectedValueChanged += delegate
        {
            if (_fillingDropDown) return;

            // Apply the text for the previous language before switching
            CommitPendingChanges();
            _selectedLanguage = _dropDownLanguage.SelectedValue as CultureInfo;
            FillTextControl();
        };

        TextControl.LostFocus += delegate { CommitPendingChanges(); };
        if (!multiline)
        {
            TextControl.KeyDown += (_, e) =>
            {
                if (e.Key == Keys.Enter) CommitPendingChanges();
            };
        }
        OnCommitPendingChanges += ApplyValue;

        OnRefresh += () =>
        {
            FillDropDown();
            FillTextControl();
        };

        Content = new TableLayout
        {
            Spacing = new(4, 0),
            Rows = {new TableRow(new TableCell(new TableLayout(_dropDownLanguage, null)), new TableCell(TextControl, scaleWidth: true))}
        };
    }

    private void FillDropDown()
    {
        var setLanguages = new List<CultureInfo>();
        var unsetLanguages = new List<CultureInfo>();
        foreach (var language in Languages.AllKnown)
        {
            if (Target != null && Target.ContainsExactLanguage(language)) setLanguages.Add(language);
            else unsetLanguages.Add(language);
        }

        _selectedLanguage ??= setLanguages is [var x] ? x : LocalizableString.DefaultLanguage;

        _fillingDropDown = true;
        try
        {
            _dropDownLanguage.DataStore = setLanguages.Concat(unsetLanguages).Cast<object>().ToList();
            _dropDownLanguage.SelectedValue = _selectedLanguage;
        }
        finally
        {
            _fillingDropDown = false;
        }
    }

    private void FillTextControl()
        => TextControl.Text = _selectedLanguage == null ? "" : Target?.GetExactLanguage(_selectedLanguage) ?? "";

    private void ApplyValue()
    {
        string? newValue = string.IsNullOrEmpty(TextControl.Text) ? null : TextControl.Text;

        if (_selectedLanguage == null || Target == null || Target.GetExactLanguage(_selectedLanguage) == newValue) return;

        if (CommandExecutor == null) Target.Set(_selectedLanguage, newValue);
        else CommandExecutor.Execute(new SetLocalizableString(Target, new LocalizableString {Language = _selectedLanguage, Value = newValue}));
    }
}
