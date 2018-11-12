// Copyright Bastian Eicher
// Licensed under the MIT License

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
            InitializeComponent();

            _editor.Open(CommandManager.For(SampleData.AddressBook));
        }

        private void InitializeComponent()
        {
            Text = "Structure Editor Sample";
            Size = new Size(800, 600);

            FormClosing += (sender, e) => { e.Cancel = !_editor.Closing(); };

            var buttonOpen = new ToolStripMenuItem {Text = "&Open...", ShortcutKeys = Keys.Control | Keys.O};
            buttonOpen.Click += delegate { _editor.Open(); };

            var buttonSave = new ToolStripMenuItem {Text = "&Save...", ShortcutKeys = Keys.Control | Keys.S};
            buttonSave.Click += delegate { _editor.Save(); };

            var buttonSaveAs = new ToolStripMenuItem {Text = "Save &As..."};
            buttonSaveAs.Click += delegate { _editor.SaveAs(); };

            var buttonUndo = new ToolStripMenuItem {Text = "&Undo", ShortcutKeys = Keys.Control | Keys.Z};
            buttonUndo.Click += delegate { _editor.Undo(); };

            var buttonRedo = new ToolStripMenuItem {Text = "&Redo", ShortcutKeys = Keys.Control | Keys.Y};
            buttonRedo.Click += delegate { _editor.Redo(); };

            Controls.Add(_editor);
            Controls.Add(new MenuStrip
            {
                Items =
                {
                    new ToolStripMenuItem
                    {
                        Text = "&File",
                        DropDownItems = {buttonOpen, buttonSave, buttonSaveAs}
                    },
                    new ToolStripMenuItem
                    {
                        Text = "&Edit",
                        DropDownItems = {buttonUndo, buttonRedo}
                    }
                }
            });
        }
    }
}
