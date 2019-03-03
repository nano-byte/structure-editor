// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Drawing;
using System.Windows.Forms;
using NanoByte.Common.Undo;
using NanoByte.StructureEditor.Sample.Controls;

namespace NanoByte.StructureEditor.Sample
{
    public class MainForm : Form
    {
        private readonly AddressBookEditor _editor = new AddressBookEditor
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
        }

        private void SetupComponents()
        {
            Text = "Structure Editor Sample";
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 600);
            FormClosing += (sender, e) => { e.Cancel = !_editor.Closing(); };

            Controls.Add(_editor);

            ToolStripMenuItem MenuItem(string text, Action action, Keys hotKey = Keys.None)
            {
                var button = new ToolStripMenuItem {Text = text, ShortcutKeys = hotKey};
                button.Click += delegate { action(); };
                return button;
            }

            Controls.Add(new MenuStrip
            {
                Items =
                {
                    new ToolStripMenuItem("&File")
                    {
                        DropDownItems =
                        {
                            MenuItem("&Open...", _editor.Open, hotKey: Keys.Control | Keys.O),
                            MenuItem("&Save...", () => _editor.Save(), hotKey: Keys.Control | Keys.S),
                            MenuItem("Save &As...", () => _editor.SaveAs())
                        }
                    },
                    new ToolStripMenuItem("&Edit")
                    {
                        DropDownItems =
                        {
                            MenuItem("&Undo", _editor.Undo, hotKey: Keys.Control | Keys.Z),
                            MenuItem("&Redo", _editor.Redo, hotKey: Keys.Control | Keys.Y)
                        }
                    }
                }
            });
        }
    }
}
