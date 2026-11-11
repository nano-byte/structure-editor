// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.IO;
using Eto.Forms;
using NanoByte.Common.Undo;
using NanoByte.StructureEditor.Sample.Controls;

namespace NanoByte.StructureEditor.Sample;

public class MainForm : Form
{
    private readonly AddressBookEditor _editor = new();

    public MainForm()
    {
        ClientSize = new(800, 600);
        Padding = 8;
        Content = _editor;
        Closing += (_, e) => e.Cancel = !_editor.Closing();

        void FileAction(Action action)
        {
            action();
            UpdateCaption();
        }

        var modifier = Application.Instance.CommonModifier;
        Menu = new MenuBar
        {
            Items =
            {
                new SubMenuItem
                {
                    Text = "&File",
                    Items =
                    {
                        MenuItem("&Open...", () => FileAction(_editor.Open), modifier | Keys.O),
                        MenuItem("&Save...", () => FileAction(() => _editor.Save()), modifier | Keys.S),
                        MenuItem("Save &As...", () => FileAction(() => _editor.SaveAs()))
                    }
                },
                new SubMenuItem
                {
                    Text = "&Edit",
                    Items =
                    {
                        MenuItem("&Undo", _editor.Undo, modifier | Keys.Z),
                        MenuItem("&Redo", _editor.Redo, modifier | Keys.Y)
                    }
                },
                new SubMenuItem
                {
                    Text = "F&ormat",
                    Items =
                    {
                        FormatMenuItem("&XML", XmlStructureSerializer.Instance),
                        FormatMenuItem("&JSON", JsonStructureSerializer.Instance)
                    }
                }
            }
        };

        _editor.Open(CommandManager.For(SampleData.AddressBook));
        UpdateCaption();
    }

    private static ButtonMenuItem MenuItem(string text, Action action, Keys shortcut = Keys.None)
    {
        var item = new ButtonMenuItem {Text = text, Shortcut = shortcut};
        item.Click += delegate { action(); };
        return item;
    }

    private readonly List<(CheckMenuItem item, IStructureSerializer serializer)> _formatMenuItems = [];

    /// <summary>
    /// Creates a menu item that switches the <see cref="IStructureSerializer"/> used for the text representation.
    /// </summary>
    private CheckMenuItem FormatMenuItem(string text, IStructureSerializer serializer)
    {
        var item = new CheckMenuItem {Text = text};
        item.Click += delegate
        {
            _editor.Serializer = serializer;
            UpdateCaption();
        };
        _formatMenuItems.Add((item, serializer));
        return item;
    }

    /// <summary>
    /// Shows the current file name and serialization format in the window title and checks the matching format menu item.
    /// </summary>
    private void UpdateCaption()
    {
        var path = _editor.CommandManager.Path;
        Title = $"Structure Editor Sample - {(string.IsNullOrEmpty(path) ? "Untitled" : Path.GetFileName(path))} [{_editor.Serializer.FormatName}]";

        foreach (var (item, serializer) in _formatMenuItems)
            item.Checked = ReferenceEquals(serializer, _editor.Serializer);
    }
}
