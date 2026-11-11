// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using NanoByte.Common.Undo;
using NanoByte.StructureEditor.Sample.Controls;

namespace NanoByte.StructureEditor.Sample;

public class MainForm : Form
{
    private readonly AddressBookEditor _editor = new()
    {
        Dock = DockStyle.Fill,
        Padding = new Padding(8)
    };

    public MainForm()
    {
        SuspendLayout();
        SetupComponents();
        ResumeLayout(performLayout: false);

        _editor.Open(CommandManager.For(SampleData.AddressBook));
        UpdateCaption();
    }

    private void SetupComponents()
    {
        AutoScaleDimensions = new SizeF(6F, 13F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 600);
        FormClosing += (_, e) => { e.Cancel = !_editor.Closing(); };

        Controls.Add(_editor);

        static ToolStripMenuItem MenuItem(string text, Action action, Keys hotKey = Keys.None)
        {
            var button = new ToolStripMenuItem {Text = text, ShortcutKeys = hotKey};
            button.Click += (_, _) => action();
            return button;
        }

        void FileAction(Action action)
        {
            action();
            UpdateCaption();
        }

        var formatMenu = new ToolStripMenuItem("F&ormat");
        formatMenu.DropDownItems.Add(FormatMenuItem("&XML", XmlStructureSerializer.Instance));
#if NET
        formatMenu.DropDownItems.Add(FormatMenuItem("&JSON", JsonStructureSerializer.Instance));
#endif

        Controls.Add(new MenuStrip
        {
            Items =
            {
                new ToolStripMenuItem("&File")
                {
                    DropDownItems =
                    {
                        MenuItem("&Open...", () => FileAction(_editor.Open), hotKey: Keys.Control | Keys.O),
                        MenuItem("&Save...", () => FileAction(() => _editor.Save()), hotKey: Keys.Control | Keys.S),
                        MenuItem("Save &As...", () => FileAction(() => _editor.SaveAs()))
                    }
                },
                new ToolStripMenuItem("&Edit")
                {
                    DropDownItems =
                    {
                        MenuItem("&Undo", _editor.Undo, hotKey: Keys.Control | Keys.Z),
                        MenuItem("&Redo", _editor.Redo, hotKey: Keys.Control | Keys.Y)
                    }
                },
                formatMenu
            }
        });
    }

    private readonly List<ToolStripMenuItem> _formatMenuItems = [];

    /// <summary>
    /// Creates a menu item that switches the <see cref="IStructureSerializer"/> used for the text representation.
    /// </summary>
    private ToolStripMenuItem FormatMenuItem(string text, IStructureSerializer serializer)
    {
        var item = new ToolStripMenuItem {Text = text, Tag = serializer};
        item.Click += (_, _) =>
        {
            _editor.Serializer = serializer;
            UpdateCaption();
        };
        _formatMenuItems.Add(item);
        return item;
    }

    /// <summary>
    /// Shows the current file name and serialization format in the window title and checks the matching format menu item.
    /// </summary>
    private void UpdateCaption()
    {
        var path = _editor.CommandManager.Path;
        Text = $"Structure Editor Sample - {(string.IsNullOrEmpty(path) ? "Untitled" : Path.GetFileName(path))} [{_editor.Serializer.FormatName}]";

        foreach (var item in _formatMenuItems)
            item.Checked = ReferenceEquals(item.Tag, _editor.Serializer);
    }
}